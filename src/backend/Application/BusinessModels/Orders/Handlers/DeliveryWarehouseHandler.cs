using Application.BusinessModels.Shared.Handlers;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using System;

namespace Application.BusinessModels.Orders.Handlers
{
    public class DeliveryWarehouseHandler : IFieldHandler<Order, Guid?>
    {

        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, Guid? oldValue, Guid? newValue)
        {
            if (newValue != null)
            {

                var deliveryWarehouse = _dataService.GetById<Warehouse>(newValue.Value);
                if (deliveryWarehouse != null)
                {
                    order.PickingFeatures = deliveryWarehouse.PickingFeatures;

                    if (deliveryWarehouse.PickingTypeId.HasValue)
                        order.PickingTypeId = deliveryWarehouse.PickingTypeId;

                    order.TransitDays = deliveryWarehouse.LeadtimeDays;

                    order.DeliveryAddress = deliveryWarehouse.Address;
                    order.DeliveryCity = deliveryWarehouse.City;
                    order.DeliveryRegion = deliveryWarehouse.Region;
                    order.DeliveryType = deliveryWarehouse.DeliveryType;

                    if (!order.ManualClientAvisationTime)
                    {
                        order.ClientAvisationTime = deliveryWarehouse.AvisaleTime;
                    }
                }
            }

            order.ShippingDate = order.DeliveryDate?.AddDays(0 - order.TransitDays ?? 0);
            order.OrderChangeDate = DateTime.UtcNow;
        }

        public string ValidateChange(Order order, Guid? oldValue, Guid? newValue)
        {
            return null;
        }

        public DeliveryWarehouseHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
    }
}
