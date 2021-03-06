﻿using Domain.Persistables;
using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Services;
using Domain.Services.Shippings;
using ThinkingHome.Migrator.Framework.Extensions;

namespace Application.Services.Shippings
{
    public class ShippingCalculationService : IShippingCalculationService
    {
        private readonly ICommonDataService _dataService;

        public ShippingCalculationService(ICommonDataService dataService)
        {
            _dataService = dataService;
        }


        public void RecalculateShipping(Shipping shipping, IEnumerable<Order> orders)
        {
            var tempRange = FindCommonTempRange(orders);
            decimal? downtime = orders.Any(o => o.TrucksDowntime.HasValue)
                ? orders.Sum(o => o.TrucksDowntime ?? 0)
                : (decimal?) null;
            int? palletsCount = orders.Any(o => o.PalletsCount.HasValue)
                ? orders.Sum(o => o.PalletsCount ?? 0)
                : (int?) null;
           int? confirmedPalletsCount = orders.Any(o => o.ConfirmedPalletsCount.HasValue)
                ? orders.Sum(o => o.ConfirmedPalletsCount ?? 0)
                : (int?) null;
            decimal? weight = orders.Any(o => o.WeightKg.HasValue) ? orders.Sum(o => o.WeightKg ?? 0) : (decimal?) null;
            decimal? confirmedWeight = orders.Any(o => o.ConfirmedWeightKg.HasValue)
                ? orders.Sum(o => o.ConfirmedWeightKg ?? 0)
                : (decimal?) null;

            shipping.TemperatureMin = tempRange?.Key;
            shipping.TemperatureMax = tempRange?.Value;
            shipping.PalletsCount = palletsCount;
            shipping.ConfirmedPalletsCount = confirmedPalletsCount;
            shipping.WeightKg = weight;
            shipping.ConfirmedWeightKg = confirmedWeight;
            shipping.TrucksDowntime = downtime;

            var loadingArrivalTime = orders.Select(i => i.LoadingArrivalTime).Where(i => i != null).Min();
            shipping.LoadingArrivalTime = loadingArrivalTime;

            var loadingDepartureTime = orders.Select(i => i.LoadingDepartureTime).Where(i => i != null).Min();
            shipping.LoadingDepartureTime = loadingDepartureTime;
        }

        private KeyValuePair<int, int>? FindCommonTempRange(IEnumerable<Order> orders)
        {
            if (!orders.Any() || orders.Any(o => o.TemperatureMin == null || o.TemperatureMax == null))
            {
                return null;
            }

            Order firstOrder = orders.First();
            KeyValuePair<int, int> result =
                new KeyValuePair<int, int>(firstOrder.TemperatureMin.Value, firstOrder.TemperatureMax.Value);

            foreach (Order order in orders.Skip(1))
            {
                if (order.TemperatureMin > result.Value || order.TemperatureMax < result.Key)
                {
                    return null;
                }

                result = new KeyValuePair<int, int>(Math.Max(result.Key, order.TemperatureMin.Value),
                    Math.Min(result.Value, order.TemperatureMax.Value));
            }

            return result;
        }
    }
}