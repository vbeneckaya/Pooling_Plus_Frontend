using Application.BusinessModels.Shared.Handlers;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Warehouses.Handlers
{
    public class LeadtimeDaysHandler : IFieldHandler<Warehouse, int?>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public LeadtimeDaysHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public void AfterChange(Warehouse entity, int? oldValue, int? newValue)
        {
            var validStatuses = new[] { OrderState.Draft, OrderState.Created, OrderState.Confirmed, OrderState.InShipping };
            var orders = _dataService.GetDbSet<Order>()
                                     .Where(x => x.SoldTo == entity.SoldToNumber
                                                && validStatuses.Contains(x.Status)
                                                && (x.ShippingId == null || x.OrderShippingStatus == ShippingState.ShippingCreated))
                                     .ToList();

            foreach (var order in orders)
            {
                if (order.TransitDays != newValue)
                {
                    _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                     nameof(order.TransitDays).ToLowerFirstLetter(),
                                                     order.TransitDays, newValue);
                    order.TransitDays = newValue;
                }

                if (!order.ManualShippingDate && !order.ManualDeliveryDate && !string.IsNullOrEmpty(order.Source))
                {
                    DateTime? newShippingDate = order.DeliveryDate?.AddDays(0 - order.TransitDays ?? 0);
                    if (order.ShippingDate != newShippingDate)
                    {
                        _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                         nameof(order.ShippingDate).ToLowerFirstLetter(),
                                                         order.ShippingDate, newShippingDate);
                        order.ShippingDate = newShippingDate;
                    }
                }
            }
        }

        public string ValidateChange(Warehouse entity, int? oldValue, int? newValue)
        {
            return null;
        }
    }
}
