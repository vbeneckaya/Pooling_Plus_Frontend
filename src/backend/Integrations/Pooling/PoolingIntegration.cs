﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DAL.Services;
using Domain.Persistables;
using Integrations.Dtos;
using Integrations.Pooling.Dtos;
using Integrations.Pooling.Enums;
using Newtonsoft.Json.Linq;

namespace Integrations.Pooling
{
    public class PoolingIntegration : ConnectorBase
    {
        private string _companyId;

        public PoolingIntegration(User user, ICommonDataService dataService) : 
            base("https://staging.k8s.devlogics.ru/api/", 
                user.PoolingLogin, 
                user.PoolingPassword, 
                dataService)
        {
            var identityData = Post("identity/login", new
            {
                username = user.PoolingLogin, 
                password = user.PoolingPassword
            });
            
            _accessToken = identityData
                .Get("accessToken");
            
            _companyId = identityData
                .Get("$.userData.companyId");
        }

        public PoolingInfoDto GetInfoFor(Shipping shipping)
        {
            var filters = Get<PoolingCalendarFilters>("slots/getFilters");

            var transportCompany = _dataService.GetById<TransportCompany>(shipping.CarrierId.Value);
            var carrier = filters.Carriers.First(x => x.Name == transportCompany.Title);

            var bodyTypeId = _dataService.GetById<VehicleType>(shipping.VehicleTypeId.Value).BodyTypeId;

            var bodyType = _dataService.GetById<BodyType>(bodyTypeId.Value);

            var carType = filters.CarTypes.First(x => x.Name == bodyType.Name);

            var orders = _dataService.GetDbSet<Order>().Where(x => x.ShippingId.Value == shipping.Id).ToList().OrderBy(x=>x.ShippingDate);
            var deliveryDate = orders.Where(x=>x.DeliveryDate != null).Max(x=>x.DeliveryDate.Value);

            var firstOrder = orders.First();
            
            var firstClientForShipping = _dataService.GetById<Client>(firstOrder.ClientId.Value);
            
            var client = filters.Clients.First(x => x.Name == firstClientForShipping.Name);
            
            var warehouseFrom = _dataService.GetById<ShippingWarehouse>(firstOrder.ShippingWarehouseId.Value);
            var warehouseTo = _dataService.GetById<Warehouse>(firstOrder.DeliveryWarehouseId.Value);

            var regionFromOrder = filters.Regions
                .First(x=> x.IsAvailable && x.Name == warehouseFrom.Region);

            var companyWarehouses = GetArr($"Definitions/warehouses?companyId={_companyId}&regionId={regionFromOrder.Id}")
                .Get($"$[?(@.name=='{warehouseFrom.WarehouseName}')].id");

            var carrieryWarehouses = GetArr($"Definitions/warehouses?companyId={carrier.Id}&regionId={regionFromOrder.Id}")
                .Get($"$[?(@.name=='{warehouseFrom.WarehouseName}')].id");

            if (!carrieryWarehouses.Any() && !companyWarehouses.Any())
                throw new Exception($"Не удалось найти склад  отгрузки '{warehouseFrom.WarehouseName}' на pooling.me");

            var warehouseId = companyWarehouses.Any() ? companyWarehouses.ElementAt(0).Value<string>() : 
                carrieryWarehouses.ElementAt(0).Value<string>();
            
            
            var slots = GetArr($"Slots/GetList?dateFrom=2020-02-04&dateTo=2020-03-04&shippingRegionId={regionFromOrder.Id}&carType={carType.Id}&productType={filters.ProductTypes.First().Id}&clientId={client.Id}&carrierId={carrier.Id}");
            var enumerable1 = slots.Get($"$[?(@.distributionCenterName=='{warehouseTo.WarehouseName}' && @.deliveryDate=='{deliveryDate.ToString("yyyy-MM-dd")}')]");

            return new PoolingInfoDto
            {
                IsAvailable = enumerable1.Any(),
                MessageField = enumerable1.Any() ? "Эту перевозку можно отправить в Pooling" : "Нет доступного слота для этой заявки",
                SlotId = enumerable1.Any() ? enumerable1.First().SelectToken("id").ToString() : null,
                WarehouseId = warehouseId
            };
        }

        public CreateReservationResult CreateReservation(Shipping shipping)
        {
            var poolingInfo = GetInfoFor(shipping);
            //throw new Exception("Не реализовано");
            var orders = _dataService.GetDbSet<Order>().Where(x => x.ShippingId.Value == shipping.Id).ToList().OrderBy(x=>x.ShippingDate);

            var sum = (int)orders.Where(x=>x.OrderAmountExcludingVAT != null).Sum(x=>x.OrderAmountExcludingVAT);
            var i = (int)orders.Where(x=>x.WeightKg != null).Sum(x=>x.WeightKg);
            var result = Post("servations/save-many", new List<object>()
            {
                new
                {
                    slotId = poolingInfo.SlotId,
                    palletCount = shipping.PalletsCount,
                    cost = sum,
                    weight = i,
                    onChangeErrors = new
                    {
                        palletCount = false,
                        cost = false,
                        weight = false,
                    },
                    id = (string)null,
                    type = shipping.TarifficationType.ToString().ToUpper(),
                    warehouseId = poolingInfo.WarehouseId,
                    date = orders.First().ShippingDate.Value.ToString("yyyy-MM-dd"),
                    timeFrom = "10:00",
                    timeTo = "18:00",
                }
            });
            return new CreateReservationResult
            {
                ReservationNumber = result.Get("$[0].slot.reservations[0].number"), 
                ReservationId = result.Get("$[0].slot.reservations[0].id"), 
            };
        }

        public string CancelReservation(Shipping shipping)
        {
            Delete($"reservations/{shipping.PoolingReservationId}");
            return "";
        }
    }
}