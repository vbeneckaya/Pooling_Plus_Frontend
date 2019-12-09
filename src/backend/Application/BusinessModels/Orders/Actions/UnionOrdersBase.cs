using Application.Shared;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.BusinessModels.Orders.Actions
{
    public abstract class UnionOrdersBase
    {
        protected readonly ICommonDataService _dataService;

        public UnionOrdersBase(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        protected KeyValuePair<int, int>? FindCommonTempRange(IEnumerable<Order> orders)
        {
            if (!orders.Any() || orders.Any(o => o.TemperatureMin == null || o.TemperatureMax == null))
            {
                return null;
            }

            Order firstOrder = orders.First();
            KeyValuePair<int, int> result = new KeyValuePair<int, int>(firstOrder.TemperatureMin.Value, firstOrder.TemperatureMax.Value);

            foreach (Order order in orders.Skip(1))
            {
                if (order.TemperatureMin > result.Value || order.TemperatureMax < result.Key)
                {
                    return null;
                }
                result = new KeyValuePair<int, int>(Math.Max(result.Key, order.TemperatureMin.Value), Math.Min(result.Value, order.TemperatureMax.Value));
            }

            return result;
        }

        protected void UnionOrderInShipping(IEnumerable<Order> allOrders, IEnumerable<Order> newOrders, Shipping shipping, IHistoryService historyService)
        {
            var tempRange = FindCommonTempRange(allOrders);
            decimal? downtime = allOrders.Any(o => o.TrucksDowntime.HasValue)
                ? allOrders.Sum(o => o.TrucksDowntime ?? 0)
                : (decimal?) null;
            int? palletsCount = allOrders.Any(o => o.PalletsCount.HasValue) ? allOrders.Sum(o => o.PalletsCount ?? 0) : (int?) null;
            int? actualPalletsCount = allOrders.Any(o => o.ActualPalletsCount.HasValue)
                ? allOrders.Sum(o => o.ActualPalletsCount ?? 0)
                : (int?) null;
            int? confirmedPalletsCount = allOrders.Any(o => o.ConfirmedPalletsCount.HasValue)
                ? allOrders.Sum(o => o.ConfirmedPalletsCount ?? 0)
                : (int?) null;
            decimal? weight = allOrders.Any(o => o.WeightKg.HasValue) ? allOrders.Sum(o => o.WeightKg ?? 0) : (decimal?) null;
            decimal? actualWeight = allOrders.Any(o => o.ActualWeightKg.HasValue)
                ? allOrders.Sum(o => o.ActualWeightKg ?? 0)
                : (decimal?) null;

            var setter = new FieldSetter<Shipping>(shipping, historyService);

            setter.UpdateField(s => s.TemperatureMin, tempRange?.Key);
            setter.UpdateField(s => s.TemperatureMax, tempRange?.Value);
            setter.UpdateField(s => s.PalletsCount, palletsCount);
            setter.UpdateField(s => s.ActualPalletsCount, actualPalletsCount);
            setter.UpdateField(s => s.ConfirmedPalletsCount, confirmedPalletsCount);
            setter.UpdateField(s => s.WeightKg, weight);
            setter.UpdateField(s => s.ActualWeightKg, actualWeight);
            setter.UpdateField(s => s.TrucksDowntime, downtime);

            var loadingArrivalTime = allOrders.Select(i => i.LoadingArrivalTime).Where(i => i != null).Min();
            setter.UpdateField(s => s.LoadingArrivalTime, loadingArrivalTime);

            var loadingDepartureTime = allOrders.Select(i => i.LoadingDepartureTime).Where(i => i != null).Min();
            setter.UpdateField(s => s.LoadingDepartureTime, loadingDepartureTime);

            foreach (var order in allOrders)
            {
                order.ShippingNumber = shipping.ShippingNumber;
                order.OrderShippingStatus = shipping.Status;
            }
            
            foreach (var order in newOrders)
            {
                order.ShippingId = shipping.Id;
                order.Status = OrderState.InShipping;

                var ordSetter = new FieldSetter<Order>(order, historyService);

                ordSetter.UpdateField(o => o.ShippingStatus, VehicleState.VehicleWaiting);
                ordSetter.UpdateField(o => o.DeliveryStatus, VehicleState.VehicleEmpty);
                ordSetter.UpdateField(o => o.CarrierId, shipping.CarrierId, nameLoader: GetCarrierNameById);

                historyService.Save(order.Id, "orderSetInShipping", order.OrderNumber, shipping.ShippingNumber);
                ordSetter.SaveHistoryLog();

                historyService.Save(shipping.Id, "shippingAddOrder", order.OrderNumber, shipping.ShippingNumber);
            }

            setter.SaveHistoryLog();
        }

        private string GetCarrierNameById(Guid? id)
        {
            return id == null ? null : _dataService.GetById<TransportCompany>(id.Value)?.Title;
        }
    }
}