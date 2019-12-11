using Application.BusinessModels.Shared.Handlers;
using Application.Services.Addresses;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Warehouses.Handlers
{
    public class AddressHandler : IFieldHandler<Warehouse, string>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly ICleanAddressService _cleanAddressService;
        private readonly bool _isManual;

        public AddressHandler(ICommonDataService dataService, IHistoryService historyService, ICleanAddressService cleanAddressService, bool isManual)
        {
            _dataService = dataService;
            _historyService = historyService;
            _cleanAddressService = cleanAddressService;
            _isManual = isManual;
        }

        public void AfterChange(Warehouse entity, string oldValue, string newValue)
        {
            string rawAddress = _isManual ? newValue : $"{entity.City} {newValue}";
            var cleanAddress = string.IsNullOrEmpty(newValue) ? null : _cleanAddressService.CleanAddress(rawAddress);
            entity.ValidAddress = cleanAddress?.ResultAddress;
            entity.PostalCode = cleanAddress?.PostalCode;
            entity.Region = cleanAddress?.Region;
            entity.Area = cleanAddress?.Area;
            if (_isManual)
            {
                entity.City = cleanAddress?.City ?? cleanAddress?.Region;
            }
            entity.Street = cleanAddress?.Street;
            entity.House = cleanAddress?.House;
            entity.UnparsedAddressParts = cleanAddress?.UnparsedAddressParts;

            var validStatuses = new[] { OrderState.Draft, OrderState.Created, OrderState.Confirmed, OrderState.InShipping };
            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.SoldTo == entity.SoldToNumber
                                                && x.DeliveryAddress != newValue
                                                && validStatuses.Contains(x.Status)
                                                && (x.ShippingId == null || x.OrderShippingStatus == ShippingState.ShippingCreated))
                                     .ToList();

            foreach (var order in orders)
            {
                _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                 nameof(order.DeliveryAddress).ToLowerFirstLetter(),
                                                 order.DeliveryAddress, newValue);
                order.DeliveryAddress = newValue;

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

        public string ValidateChange(Warehouse entity, string oldValue, string newValue)
        {
            return null;
        }
    }
}
