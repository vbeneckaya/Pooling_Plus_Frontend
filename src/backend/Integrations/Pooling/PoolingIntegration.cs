using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.Clients;
using Domain.Services.Orders;
using Domain.Services.Shippings;
using Domain.Services.ShippingWarehouses;
using Domain.Services.TransportCompanies;
using Domain.Services.UserProvider;
using Domain.Services.Warehouses;
using Domain.Shared;
using Integrations.Pooling.Dtos;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Integrations.Pooling
{
    public class PoolingIntegration : ConnectorBase
    {
        public static readonly string NeedLoginPasswordMessage = "Укажите доступ к pooling.me в настройках профиля";
        private string _companyId;
        private User _user;
        private PoolingInfoDto _poolingInfoDto;
        private IServiceProvider _serviceProvider;

        public PoolingIntegration(
            User user,
            ICommonDataService dataService,
            IServiceProvider serviceProvider = null
        ) :
            base("https://stage.pooling.artlogics.ru/api/", //"https://staging.k8s.devlogics.ru/api/", 
                user.PoolingLogin,
                user.PoolingPassword,
                dataService)
        {
            var identityData = Put("identity/login", new
            {
                username = user.PoolingLogin,
                password = user.PoolingPassword
            });

            if (identityData != null)
            {
                _accessToken = identityData
                    .Get("accessToken");

                _companyId = identityData
                    .Get("$.userData.companyId");
            }

            _user = user;

            _serviceProvider = serviceProvider;
        }

        public PoolingInfoDto GetInfoFor(Shipping shipping)
        {
            var filters = Get<PoolingCalendarFilters>("slots/filters");

            var transportCompany = _dataService.GetById<TransportCompany>(shipping.CarrierId.Value);
            var carrier = filters.Carriers.First(x => x.Name == transportCompany.Title);

            var bodyTypeId = _dataService.GetById<VehicleType>(shipping.VehicleTypeId.Value).BodyTypeId;

            var bodyType = _dataService.GetById<BodyType>(bodyTypeId.Value);

            var carType = filters.CarTypes.First(x => x.Name == bodyType.Name);

            var orders = _dataService.GetDbSet<Order>().Where(x => x.ShippingId.Value == shipping.Id).ToList()
                .OrderBy(x => x.ShippingDate);
            var deliveryDate = orders.Where(x => x.DeliveryDate != null).Max(x => x.DeliveryDate.Value);

            var firstOrder = orders.First();

            var firstClientForShipping = _dataService.GetById<Client>(firstOrder.ClientId.Value);

            var client = filters.Clients.First(x => x.Name == firstClientForShipping.Name);

            var warehouseFrom = _dataService.GetById<ShippingWarehouse>(firstOrder.ShippingWarehouseId.Value);
            var warehouseTo = _dataService.GetById<Warehouse>(firstOrder.DeliveryWarehouseId.Value);

            var regionFromOrder = filters.Regions
                .First(x => x.IsAvailable && x.Name == warehouseFrom.Region);

            var companyWarehouses =
                GetArr($"Definitions/warehouses?companyId={_companyId}&regionId={regionFromOrder.Id}")
                    .Get($"$[?(@.name=='{warehouseFrom.WarehouseName}')].id");

            var carrieryWarehouses =
                GetArr($"Definitions/warehouses?companyId={carrier.Id}&regionId={regionFromOrder.Id}")
                    .Get($"$[?(@.name=='{warehouseFrom.WarehouseName}')].id");

            if (!carrieryWarehouses.Any() && !companyWarehouses.Any())
                throw new Exception($"Не удалось найти склад  отгрузки '{warehouseFrom.WarehouseName}' на pooling.me");

            var warehouseId = companyWarehouses.Any()
                ? companyWarehouses.ElementAt(0).Value<string>()
                : carrieryWarehouses.ElementAt(0).Value<string>();


            var slots = GetArr(
                $"slots?dateFrom=2020-02-04&dateTo=2020-03-04&shippingRegionId={regionFromOrder.Id}&carType={carType.Id}&productType={filters.ProductTypes.First().Id}&clientId={client.Id}&carrierId={carrier.Id}");
            var enumerable1 =
                slots.Get(
                    $"$[?(@.distributionCenterName=='{warehouseTo.WarehouseName}' && @.deliveryDate=='{deliveryDate.ToString("yyyy-MM-dd")}')]");

            _poolingInfoDto = new PoolingInfoDto
            {
                IsAvailable = enumerable1.Any(),
                MessageField = enumerable1.Any()
                    ? "Эту перевозку можно отправить в Pooling"
                    : "Нет доступного слота для этой перевозки",
                SlotId = enumerable1.Any() ? enumerable1.First().SelectToken("id").ToString() : null,
                WarehouseId = warehouseId
            };
            return _poolingInfoDto;
        }

        public CreateReservationResult CreateReservation(Shipping shipping)
        {
            var orders = _dataService.GetDbSet<Order>().Where(x => x.ShippingId.Value == shipping.Id).ToList()
                .OrderBy(x => x.ShippingDate);

            var cost = (int) orders.Where(x => x.OrderAmountExcludingVAT != null).Sum(x => x.OrderAmountExcludingVAT);
            var weight = (int) orders.Where(x => x.WeightKg != null).Sum(x => x.WeightKg);

            var ordersForSent = new List<object>();
            var currentPalletCount = 0;
            foreach (var order in orders)
            {
                ordersForSent.Add(new
                {
                    orderNumber = order.ClientOrderNumber,
                    packingList = order.OrderNumber,
                    onChangeErrors = new
                    {
                        orderNumber = false,
                        packingList = false,
                        palletTo = false
                    },
                    palletFrom = currentPalletCount + 1,
                    palletTo = currentPalletCount + order.PalletsCount,
                    type = 0
                });
                currentPalletCount = (int) (currentPalletCount + order.PalletsCount);
            }

            var tarifficationType = shipping.TarifficationType.ToString().ToUpper();
            var date = orders.First().ShippingDate.Value.ToString("yyyy-MM-dd");
            var poolingInfoWarehouseId = _poolingInfoDto.WarehouseId;

            try
            {
                var result = PostArr("reservations/save-many", new List<object>()
                {
                    new
                    {
                        slotId = _poolingInfoDto.SlotId,
                        palletCount = shipping.PalletsCount,
                        cost = cost,
                        weight = weight,
                        onChangeErrors = new
                        {
                            palletCount = false,
                            cost = false,
                            weight = false,
                        },
                        orders = ordersForSent,
                        id = (string) null,
                        type = tarifficationType,
                        warehouseId = poolingInfoWarehouseId,
                        date = date,
                        timeFrom = "10:00",
                        timeTo = "18:00",
                    }
                });

                var createReservationResult = new CreateReservationResult();

                try
                {
                    createReservationResult.Error = result.Get("$[0].error").ElementAt(0).Value<string>();
                }
                catch (Exception e)
                {
                }

                if (!string.IsNullOrEmpty(createReservationResult.Error))
                    return createReservationResult;

                createReservationResult.ReservationId =
                    result.Get("$[0].slot.reservations[0].id").ElementAt(0).ToString();
                createReservationResult.ReservationNumber =
                    result.Get("$[0].slot.reservations[0].number").ElementAt(0).ToString();
                return createReservationResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string CancelReservation(Shipping shipping)
        {
            Delete($"reservations/{shipping.PoolingReservationId}");
            return "";
        }

        public string Update(Shipping shipping)
        {
            Get($"reservations/{shipping.PoolingReservationId}")
                .Get("$.slot.palletLeft");
            //shipping.PoolingInfo = "";
            return "";
        }

        public void Init()
        {
            LoadShippingWarehouses();
        }


        public void LoadShippingsAndOrdersFromReports(DateTime startDate, DateTime endDate, string providerId = null)
        {
            var shippingWarehousesService = _serviceProvider.GetRequiredService<IShippingWarehousesService>();
            var warehousesService = _serviceProvider.GetService<IWarehousesService>();
            var shippingsService = _serviceProvider.GetService<IShippingsService>();
            var ordersService = _serviceProvider.GetService<IOrdersService>();
            var clientsService = _serviceProvider.GetService<IClientsService>();
            var transportCompaniesService = _serviceProvider.GetService<ITransportCompaniesService>();
            var calcService = _serviceProvider.GetService<IDeliveryCostCalcService>();
            var dataService = _serviceProvider.GetService<ICommonDataService>();

            var firstAdministrator = new CurrentUserDto
            {
                Id = dataService.GetDbSet<User>()
                    .FirstOrDefault(_ => _.IsActive && _.Role.RoleType == RoleTypes.Administrator)?.Id
            };

            try
            {
                var consignors = GetArr($"definitions/consignors");

                var providersIntegratedWithPooling = dataService.GetDbSet<Provider>()
                    .Where(_ => _.IsActive && _.IsPoolingIntegrated &&
                                providerId == null || _.Id == Guid.Parse(providerId)).Select(_ =>
                        new
                        {
                            _.Id,
                            _.Name
                        })
                    .AsQueryable();

                foreach (var provider in providersIntegratedWithPooling)
                {
                    var consignorId = consignors.Get($"$[?(@.name=='{provider.Name}')].id").FirstOrDefault();

                    if (consignorId == null) continue;

                    var reservationsFromPooling =
                        Get<JObject>(
                                $"generalReport?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&consignorId={consignorId}&showAll=true")
                            .GetValue("reservations");

                    foreach (var reservation in reservationsFromPooling)
                    {
                        var ordersIds = new List<Guid>();
                        var shippingId = Guid.Empty;
                        try
                        {
                            var poolingReservationId = reservation.Value<string>("id");
                            
                            var carType = reservation.Value<string>("carType");

                            var ordersNumbers = reservation.Value<string>("packingLists").Replace(" ", "")
                                .Split(",");

                            var orderClientNumbers =
                                reservation.Value<string>("orderNumbers").Replace(" ", "").Split(",");

                            var shippingWarehouseId = _dataService.GetDbSet<ShippingWarehouse>()
                                .FirstOrDefault(_ => _.ProviderId == Guid.Parse(providerId)
                                                     && _.WarehouseName == reservation.Value<string>("loadingPlace"))
                                ?.Id ;

                            if (shippingWarehouseId == null)
                            {
                                var newShippingWarehouse = new ShippingWarehouse
                                {
                                    Id = Guid.NewGuid(),
                                    ProviderId = Guid.Parse(providerId),
                                    WarehouseName = reservation.Value<string>("loadingPlace")
                                };
                                _dataService.GetDbSet<ShippingWarehouse>().Add(newShippingWarehouse);
                                shippingWarehouseId = newShippingWarehouse.Id;
                            }

                            var deliveryWarehouseName = reservation.Value<string>("distributionCenter");

                            Int32.TryParse(reservation.Value<string>("palletCount"), out var palletsCount);

                            Int32.TryParse(reservation.Value<string>("weight"), out var weight);

                            DateTime.TryParseExact(
                                reservation.Value<string>("deliveryToTheWarehouseDateTime").Split(" ")[0],
                                "dd.MM.yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var shippingDate);

                            DateTime.TryParseExact(reservation.Value<string>("createAt"), "dd.MM.yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var createDate);

                            DateTime.TryParseExact(reservation.Value<string>("dateOfAcceptanceByTheConsignee"),
                                "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                                out var deliveryDate);

                            var carrierCompanyName = reservation.Value<string>("carrierCompany");
                            Guid.TryParse(transportCompaniesService?.ForSelect()
                                .FirstOrDefault(_ => _.Name == carrierCompanyName)
                                .Value, out var carrierId);


                            var clientName = reservation.Value<string>("client");
                            Guid.TryParse(clientsService?.ForSelect()
                                .FirstOrDefault(_ => _.Name == clientName)?.Value, out var clientId);

                            Guid? deliveryWarehouseId = null;
                            if (clientId != null)
                            {
                                var deliveryWarehouse = warehousesService?.ForSelect(clientId)
                                    .FirstOrDefault(_ => _.Name == deliveryWarehouseName);
                                deliveryWarehouseId = deliveryWarehouse == null
                                    ? (Guid?) null
                                    : Guid.Parse(deliveryWarehouse.Value);
                            }

                            var deliveryType = reservation.Value<string>("deliveryType");

                            decimal.TryParse(reservation.Value<string>("totalPrice"), out var deliveryCost);

                            decimal.TryParse(reservation.Value<string>("cost"), out var invoiceCost);

                            var shipping = _dataService.GetDbSet<Shipping>()
                                               .FirstOrDefault(_ => _.PoolingReservationId == poolingReservationId) ??
                                           new Shipping {PoolingReservationId = poolingReservationId};

                            shipping.ShippingCreationDate = createDate;

                            shipping.ProviderId = provider.Id;

                            shipping.CarrierId = carrierId;

                            switch (deliveryType)
                            {
                                case "LTL":
                                    shipping.TarifficationType = TarifficationType.Ltl;
                                    break;
                                default:
                                    shipping.TarifficationType = null;
                                    break;
                            }

                            shipping.VehicleTypeId = _dataService.GetDbSet<VehicleType>()
                                .FirstOrDefault(_ => string.Equals(_.Name, carType, StringComparison.CurrentCultureIgnoreCase))?.Id;
                            
                            shipping.InvoiceAmount = invoiceCost;
                            

                            shipping.PalletsCount = palletsCount;

                            shipping.WeightKg = weight;

                            var isNewShipping = shipping.Id == Guid.Empty;

                            if (isNewShipping)
                            {
                                shipping.Id = Guid.NewGuid();

                                shipping.UserCreatorId = _user.Id;

                                shippingsService.InitializeNewShipping(shipping, firstAdministrator);

                                _dataService.GetDbSet<Shipping>().Add(shipping);
                            }
                            else
                            {
                                _dataService.GetDbSet<Shipping>().Update(shipping);
                            }
                            
                            _dataService.SaveChanges();

                            shippingId = shipping.Id;

                            ordersIds = new List<Guid>();

                            palletsCount = palletsCount - ordersNumbers.Length;

                            decimal? distributedWeight = 0;

                            var loop = 0;

                            foreach (var orderNumber in ordersNumbers)
                            {
                                loop++;

                                var ordersSearch = ordersService.FindByNumberAndProvider(new NumberSearchFormDto
                                {
                                    ProviderId = provider.Id.ToString(),
                                    Number = orderNumber
                                }).FirstOrDefault();

                                var order = ordersSearch == null
                                    ? new Order {OrderNumber = orderNumber}
                                    : _dataService.GetById<Order>(Guid.Parse(ordersSearch.Value));

                                order.OrderCreationDate = createDate;

                                order.ProviderId = shipping.ProviderId;

                                order.ShippingWarehouseId = shippingWarehouseId;

                                order.ClientId = clientId;

                                order.DeliveryWarehouseId = order.ClientId == null
                                    ? null
                                    : deliveryWarehouseId;

                                order.ClientOrderNumber = orderClientNumbers[ordersNumbers.IndexOf(orderNumber)];

                                order.ShippingId = shipping.Id;

                                order.DeliveryDate = deliveryDate;

                                order.ShippingDate = shippingDate;

                                order.PalletsCount = ++palletsCount;

                                order.WeightKg = loop == ordersNumbers.Length
                                    ? weight - distributedWeight
                                    : weight / ordersNumbers.Length;

                                distributedWeight += order.WeightKg;
                                palletsCount = 0;

                                if (order.Id == Guid.Empty)
                                {
                                    order.Id = Guid.NewGuid();

                                    ordersService.InitializeNewOrder(order);

                                    _dataService.GetDbSet<Order>().Add(order);
                                }
                                else
                                {
                                    _dataService.GetDbSet<Order>().Update(order);
                                }

                                _dataService.SaveChanges();

                                ordersIds.Add(order.Id);
                            }

                            if (ordersIds.Any())
                            {
                                if (isNewShipping)
                                {
                                    ordersService.InvokeAction("unionOrdersInExisted", ordersIds);

                                    if (deliveryType.Equals("Pooling"))
                                    {
                                        shipping.TotalDeliveryCost = deliveryCost;
                                        _dataService.GetDbSet<Shipping>().Update(shipping);
                                        _dataService.SaveChanges();
                                    }
                                }

                                if ((shipping.TotalDeliveryCost == null || shipping.TotalDeliveryCost == 0) &&
                                    shipping.TarifficationType != null)
                                {
                                    calcService.UpdateDeliveryCost(_dataService.GetById<Shipping>(shipping.Id), true);
                                }
                            }

                            if (deliveryDate < DateTime.Today)
                            {
                                SetStatusToComplete(shipping.Id);
                            }
                            
                            

                        }
                        catch (Exception e)
                        {
                            RollBackOneRowFromReports(shippingId, ordersIds);
                            throw;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void SetStatusToComplete(Guid shippingId)
        {
            var shipping = _dataService.GetById<Shipping>(shippingId);
            shipping.Status = ShippingState.ShippingCompleted;
            var orders = _dataService.GetDbSet<Order>().Where(_ => _.ShippingId == shippingId).AsEnumerable()
                .Select(SetOrderToComplete);

            _dataService.GetDbSet<Shipping>().Update(shipping);
            _dataService.GetDbSet<Order>().UpdateRange(orders);
        }

        private Order SetOrderToComplete(Order order)
        {
            order.Status = OrderState.Delivered;
            order.OrderShippingStatus = ShippingState.ShippingCompleted;
            return order;
        }

        private void RollBackOneRowFromReports(Guid shippingId, IEnumerable<Guid> ordersIds)
        {
            var shippingsService = _serviceProvider.GetService<IShippingsService>();
            var ordersService = _serviceProvider.GetService<IOrdersService>();

            shippingsService.InvokeAction("deleteShipping", new[] {shippingId});
            ordersService.InvokeAction("deleteOrder", ordersIds);
        }

        private void LoadShippingWarehouses()
        {
            if (!_user.ProviderId.HasValue) return;

            var _shippingWarehousesService = _serviceProvider.GetRequiredService<IShippingWarehousesService>();

            var warehousesFromPooling = GetArr($"definitions/warehouses?companyId={_companyId}").Get("$")
                .FirstOrDefault()?.ToList();
            var regionsFromPooling = Get<JObject>("slots/filters").GetValue("regions").ToList();

            foreach (var warehouse in warehousesFromPooling)
            {
                var regionId = warehouse.Value<string>("regionId");
                var name = warehouse.Value<string>("name");
                var region = regionsFromPooling
                    .FirstOrDefault(_ => _.Value<string>("id") == regionId)?.Value<string>("name");
                var address = warehouse["addressInfo"].Value<string>("address");

                var existedShippingWarehouse =
                    _shippingWarehousesService.GetByNameAndProviderId(name,
                        _user.ProviderId.Value) ?? new ShippingWarehouseDto
                    {
                        WarehouseName = name
                    };

                existedShippingWarehouse.Region = region;
                existedShippingWarehouse.Address = address;

                _shippingWarehousesService.SaveOrCreate(existedShippingWarehouse);
            }
        }
    }
}