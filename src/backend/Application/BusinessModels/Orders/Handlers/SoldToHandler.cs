using Application.BusinessModels.Shared.Handlers;
using Application.Shared;
using DAL.Queries;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class SoldToHandler : IFieldHandler<Order, string>
    {

        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public void AfterChange(Order order, string oldValue, string newValue)
        {
            if (!string.IsNullOrEmpty(order.SoldTo))
            {

                var soldToWarehouse = _dataService.GetDbSet<Warehouse>().FirstOrDefault(x => x.SoldToNumber == order.SoldTo);
                if (soldToWarehouse != null)
                {
                    if (soldToWarehouse.PickingTypeId.HasValue)
                        order.PickingTypeId = soldToWarehouse.PickingTypeId;

                    order.TransitDays = soldToWarehouse.LeadtimeDays;

                    order.DeliveryWarehouseId = soldToWarehouse.Id;
                    order.DeliveryAddress = soldToWarehouse.Address;
                    order.DeliveryCity = soldToWarehouse.City;
                    order.DeliveryRegion = soldToWarehouse.Region;
                    order.DeliveryType = soldToWarehouse.DeliveryType;

                    if (!order.ManualClientAvisationTime)
                    {
                        order.ClientAvisationTime = soldToWarehouse.AvisaleTime;
                    }
                }
                else
                {
                    order.DeliveryWarehouseId = null;
                }
            }

            order.ShippingDate = order.DeliveryDate?.AddDays(0 - order.TransitDays ?? 0);
            order.OrderChangeDate = DateTime.UtcNow;
        }

        public string ValidateChange(Order order, string oldValue, string newValue)
        {
            return null;
        }

        private string GetPickingTypeNameById(Guid? id)
        {
            return id == null ? null : _dataService.GetDbSet<PickingType>().GetById(id.Value)?.Name;
        }

        public SoldToHandler(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }
    }
}
