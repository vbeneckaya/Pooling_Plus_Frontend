using System;
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
            base("https://stage.pooling.artlogics.ru/api/", 
                user.PoolingLogin, 
                user.PoolingPassword, 
                dataService)
        {
            var identityData = Put("identity/login", new PoolingIdentityLoginParams(user.PoolingLogin, user.PoolingPassword));
            
            _accessToken = identityData
                .Get("accessToken");
            
            _companyId = identityData
                .Get("$.userData.companyId");
        }

        public bool IsAvaliable(Shipping shipping)
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

            var enumerable = GetArr($"Definitions/warehouses?companyId={_companyId}&regionId={regionFromOrder.Id}")
                .Get($"$[?(@.name=='{warehouseFrom.WarehouseName}')].id");
            var warehouseId = enumerable.ElementAt(0).Value<string>();
            
            
            var slots = GetArr($"Slots/GetList?dateFrom=2020-02-04&dateTo=2020-03-04&shippingRegionId={regionFromOrder.Id}&carType={carType.Id}&productType={filters.ProductTypes.First().Id}&clientId={client.Id}&carrierId={carrier.Id}");
            var enumerable1 = slots.Get($"$[?(@.distributionCenterName=='{warehouseTo.WarehouseName}' && @.deliveryDate=='{deliveryDate.ToString("yyyy-MM-dd")}')]");

            return enumerable1.Any();
        }

        public string CreateReservation(Shipping shipping)
        {
            throw new Exception("Не реализовано");
            var result = Post<PoolingCreateReservetionAnswer>("/reservations/create", new CreateReservetionDto{});
            return result.Number;
        }
    }
}