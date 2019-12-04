using System;
using System.Collections.Generic;
using System.Linq;
using Application.Shared;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.History;
using Microsoft.EntityFrameworkCore;

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

        protected void UnionOrderInShipping(IEnumerable<Order> orders, Shipping shipping, DbSet<Shipping> shippingDbSet, IHistoryService historyService)
        {
            var tempRange = FindCommonTempRange(orders);
            decimal? downtime = orders.Any(o => o.TrucksDowntime.HasValue)
                ? orders.Sum(o => o.TrucksDowntime ?? 0)
                : (decimal?) null;
            int? palletsCount = orders.Any(o => o.PalletsCount.HasValue) ? orders.Sum(o => o.PalletsCount ?? 0) : (int?) null;
            int? actualPalletsCount = orders.Any(o => o.ActualPalletsCount.HasValue)
                ? orders.Sum(o => o.ActualPalletsCount ?? 0)
                : (int?) null;
            int? confirmedPalletsCount = orders.Any(o => o.ConfirmedPalletsCount.HasValue)
                ? orders.Sum(o => o.ConfirmedPalletsCount ?? 0)
                : (int?) null;
            decimal? weight = orders.Any(o => o.WeightKg.HasValue) ? orders.Sum(o => o.WeightKg ?? 0) : (decimal?) null;
            decimal? actualWeight = orders.Any(o => o.ActualWeightKg.HasValue)
                ? orders.Sum(o => o.ActualWeightKg ?? 0)
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

            var loadingArrivalTime = orders.Select(i => i.LoadingArrivalTime).Where(i => i != null).Min();
            setter.UpdateField(s => s.LoadingArrivalTime, loadingArrivalTime);

            var loadingDepartureTime = orders.Select(i => i.LoadingDepartureTime).Where(i => i != null).Min();
            setter.UpdateField(s => s.LoadingDepartureTime, loadingDepartureTime);
            
            setter.SaveHistoryLog();
            
            foreach (var order in orders)
            {
                order.ShippingId = shipping.Id;
                order.ShippingNumber = shipping.ShippingNumber;
                order.Status = OrderState.InShipping;
                order.OrderShippingStatus = shipping.Status;

                var ordSetter = new FieldSetter<Order>(order, historyService);

                ordSetter.UpdateField(o => o.ShippingStatus, VehicleState.VehicleWaiting);
                ordSetter.UpdateField(o => o.DeliveryStatus, VehicleState.VehicleEmpty);
                ordSetter.UpdateField(o => o.CarrierId, shipping.CarrierId, nameLoader: GetCarrierNameById);

                historyService.Save(order.Id, "orderSetInShipping", order.OrderNumber, shipping.ShippingNumber);
                ordSetter.SaveHistoryLog();
            }
        }

        private string GetCarrierNameById(Guid? id)
        {
            return id == null ? null : _dataService.GetById<TransportCompany>(id.Value)?.Title;
        }
    }
}