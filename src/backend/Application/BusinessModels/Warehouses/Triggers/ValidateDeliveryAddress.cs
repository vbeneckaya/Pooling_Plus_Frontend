using Application.BusinessModels.Shared.Triggers;
using Application.Services.Addresses;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Shared;
using System;
using System.Linq;

namespace Application.BusinessModels.Warehouses.Triggers
{
    public class ValidateDeliveryAddress : ITrigger<Warehouse>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly ICleanAddressService _cleanAddressService;

        public ValidateDeliveryAddress(ICommonDataService dataService, IHistoryService historyService, ICleanAddressService cleanAddressService)
        {
            _dataService = dataService;
            _historyService = historyService;
            _cleanAddressService = cleanAddressService;
        }

        public void Execute(Warehouse entity)
        {
            string rawAddress = $"{entity.City} {entity.Address}";
            var cleanAddress = string.IsNullOrEmpty(rawAddress) ? null : _cleanAddressService.CleanAddress(rawAddress);

            entity.ValidAddress = cleanAddress?.ResultAddress;
            entity.PostalCode = cleanAddress?.PostalCode;
            entity.Region = cleanAddress?.Region;
            entity.Area = cleanAddress?.Area;
            entity.Street = cleanAddress?.Street;
            entity.House = cleanAddress?.House;
            entity.UnparsedAddressParts = cleanAddress?.UnparsedAddressParts;

            var validStatuses = new[] { OrderState.Created, OrderState.InShipping };
            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.DeliveryWarehouseId == entity.Id
                                                && validStatuses.Contains(x.Status)
                                                && (x.ShippingId == null || x.OrderShippingStatus == ShippingState.ShippingCreated))
                                     .ToList();

            foreach (var order in orders)
            {
                if (order.DeliveryAddress != entity.Address)
                {
                    _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                     nameof(order.DeliveryAddress).ToLowerFirstLetter(),
                                                     order.DeliveryAddress, entity.Address);
                    order.DeliveryAddress = entity.Address;
                }

                if (order.DeliveryRegion != entity.Region)
                {
                    _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                     nameof(order.DeliveryRegion).ToLowerFirstLetter(),
                                                     order.DeliveryRegion, entity.Region);
                    order.DeliveryRegion = entity.Region;
                }

                if (order.DeliveryCity != entity.City)
                {
                    _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                     nameof(order.DeliveryCity).ToLowerFirstLetter(),
                                                     order.DeliveryCity, entity.City);
                    order.DeliveryCity = entity.City;
                }
            }
        }

        public bool IsTriggered(EntityChanges<Warehouse> changes)
        {
            var watchProperties = new[]
            {
                nameof(Warehouse.City),
                nameof(Warehouse.Address)
            };
            return changes?.FieldChanges?.Count(x => watchProperties.Contains(x.FieldName)) > 0;
        }
    }
}
