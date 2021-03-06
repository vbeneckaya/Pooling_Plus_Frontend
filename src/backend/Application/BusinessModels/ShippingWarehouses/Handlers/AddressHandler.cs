﻿using Application.BusinessModels.Shared.Handlers;
using Application.Services.Addresses;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.ShippingWarehouses.Handlers
{
    public class AddressHandler : IFieldHandler<ShippingWarehouse, string>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly ICleanAddressService _cleanAddressService;

        public AddressHandler(ICommonDataService dataService, IHistoryService historyService, ICleanAddressService cleanAddressService)
        {
            _dataService = dataService;
            _historyService = historyService;
            _cleanAddressService = cleanAddressService;
        }

        public void AfterChange(ShippingWarehouse entity, string oldValue, string newValue)
        {
            var cleanAddress = string.IsNullOrEmpty(newValue) ? null : _cleanAddressService.CleanAddress(newValue);
            entity.Address = cleanAddress?.ResultAddress ?? entity.Address;
            entity.PostalCode = cleanAddress?.PostalCode;
            entity.Region = cleanAddress?.Region;
            entity.Area = cleanAddress?.Area;
            entity.City = cleanAddress?.City;
            entity.Settlement = cleanAddress?.Settlement;
            entity.Street = cleanAddress?.Street;
            entity.House = cleanAddress?.House;
            entity.Block = cleanAddress?.Block;
            entity.UnparsedParts = cleanAddress?.UnparsedAddressParts;

            var validStatuses = new[] { OrderState.Created, OrderState.InShipping };
            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.ShippingWarehouseId == entity.Id
                                                && x.ShippingAddress != entity.Address
                                                && validStatuses.Contains(x.Status)
                                                && (x.ShippingId == null || x.OrderShippingStatus == ShippingState.ShippingCreated))
                                     .ToList();

            foreach (var order in orders)
            {
                _historyService.SaveImpersonated(null, order.Id, "fieldChanged", 
                                                 nameof(order.ShippingAddress).ToLowerFirstLetter(), 
                                                 order.ShippingAddress, entity.Address);
                order.ShippingAddress = entity.Address;
            }
        }

        public string ValidateChange(ShippingWarehouse entity, string oldValue, string newValue)
        {
            return null;
        }
    }
}
