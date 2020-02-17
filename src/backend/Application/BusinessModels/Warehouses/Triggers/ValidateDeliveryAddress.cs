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

        public void Execute(Warehouse shipping)
        {
            string rawAddress = $"{shipping.City} {shipping.Address}";
            var cleanAddress = string.IsNullOrEmpty(rawAddress) ? null : _cleanAddressService.CleanAddress(rawAddress);

            shipping.ValidAddress = cleanAddress?.ResultAddress;
            shipping.PostalCode = cleanAddress?.PostalCode;
            shipping.Region = cleanAddress?.Region;
            shipping.City = cleanAddress?.City;
            shipping.Area = cleanAddress?.Area;
            shipping.Street = cleanAddress?.Street;
            shipping.House = cleanAddress?.House;
            shipping.UnparsedAddressParts = cleanAddress?.UnparsedAddressParts;

            var validStatuses = new[] { OrderState.Created, OrderState.InShipping };
            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.DeliveryWarehouseId == shipping.Id
                                                && validStatuses.Contains(x.Status)
                                                && (x.ShippingId == null || x.OrderShippingStatus == ShippingState.ShippingCreated))
                                     .ToList();

            foreach (var order in orders)
            {
                if (order.DeliveryAddress != shipping.Address)
                {
                    _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                     nameof(order.DeliveryAddress).ToLowerFirstLetter(),
                                                     order.DeliveryAddress, shipping.Address);
                    order.DeliveryAddress = shipping.Address;
                }

                if (order.DeliveryRegion != shipping.Region)
                {
                    _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                     nameof(order.DeliveryRegion).ToLowerFirstLetter(),
                                                     order.DeliveryRegion, shipping.Region);
                    order.DeliveryRegion = shipping.Region;
                }

                if (order.DeliveryCity != shipping.City)
                {
                    _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                     nameof(order.DeliveryCity).ToLowerFirstLetter(),
                                                     order.DeliveryCity, shipping.City);
                    order.DeliveryCity = shipping.City;
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
