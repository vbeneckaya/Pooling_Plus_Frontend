using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.ShippingWarehouses;
using Integrations.Pooling.Dtos;
using Newtonsoft.Json.Linq;

namespace Integrations.Pooling
{
    public class PoolingIntegration : ConnectorBase
    {
        public static readonly string NeedLoginPasswordMessage = "Укажите доступ к pooling.me в настройках профиля";
        private string _companyId;
        private User _user;
        private PoolingInfoDto _poolingInfoDto;
        private IShippingWarehousesService _shippingWarehouseService;

        public PoolingIntegration(
            User user,
            ICommonDataService dataService,
            IShippingWarehousesService shippingWarehousesService = null
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

            _accessToken = identityData
                .Get("accessToken");

            _companyId = identityData
                .Get("$.userData.companyId");

            _shippingWarehouseService = shippingWarehousesService;

            _user = user;
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
            FetchShippingWarehouses();
        }

        private void FetchShippingWarehouses()
        {
            if (!_user.ProviderId.HasValue) return;
            
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
                    _shippingWarehouseService.GetByNameAndProviderId(name,
                        _user.ProviderId.Value) ?? new ShippingWarehouseDto
                    {
                        WarehouseName = name
                    };

                existedShippingWarehouse.Region = region;
                existedShippingWarehouse.Address = address;

                _shippingWarehouseService.SaveOrCreate(existedShippingWarehouse);
            }
        }
    }
}