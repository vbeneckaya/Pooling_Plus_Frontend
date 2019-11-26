using Application.Shared;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.History;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Shippings
{
    public class DeliveryCostCalcService : IDeliveryCostCalcService
    {
        private readonly ICommonDataService _commonDataService;
        private readonly IHistoryService _historyService;

        public DeliveryCostCalcService(ICommonDataService commonDataService, IHistoryService historyService)
        {
            _commonDataService = commonDataService;
            _historyService = historyService;
        }

        public void UpdateDeliveryCost(Shipping shipping)
        {
            if (shipping.Status != ShippingState.ShippingCreated
                || shipping.CarrierId == null
                || shipping.VehicleTypeId == null
                || shipping.BodyTypeId == null
                || shipping.TarifficationType == null)
            {
                return;
            }

            var orders = _commonDataService.GetDbSet<Order>()
                                           .Where(x => x.ShippingId == shipping.Id)
                                           .ToList();

            var hasSkippingOrders = orders.Where(x => x.DeliveryType != DeliveryType.Delivery
                                                    || x.DeliveryCost != null
                                                    || x.PalletsCount == null
                                                    || x.PalletsCount <= 0
                                                    || x.ShippingDate == null
                                                    || x.DeliveryDate == null
                                                    || string.IsNullOrEmpty(x.ShippingCity)
                                                    || string.IsNullOrEmpty(x.DeliveryCity))
                                          .Any();
            if (hasSkippingOrders)
            {
                return;
            }

            foreach (var group in orders.GroupBy(x => new { x.ShippingCity, x.DeliveryCity }))
            {
                decimal deliveryCost = GetOrderGroupDeliveryCost(group.Key.ShippingCity, group.Key.DeliveryCity, shipping, group.AsEnumerable()) ?? 0M;
                foreach (var order in group)
                {
                    var setter = new FieldSetter<Order>(order, _historyService);
                    setter.UpdateField(x => x.DeliveryCost, deliveryCost);
                    setter.SaveHistoryLog();
                }
            }
        }

        private decimal? GetOrderGroupDeliveryCost(string shippingCity, string deliveryCity, Shipping shipping, IEnumerable<Order> orders)
        {
            DateTime shippingDate = orders.Min(x => x.ShippingDate.Value);

            var tariff = _commonDataService.GetDbSet<Tariff>()
                                           .Where(x => x.CarrierId == shipping.CarrierId
                                                        && x.VehicleTypeId == shipping.VehicleTypeId
                                                        && x.BodyTypeId == shipping.BodyTypeId
                                                        && x.TarifficationType == shipping.TarifficationType
                                                        && x.ShipmentCity == shippingCity
                                                        && x.DeliveryCity == deliveryCity
                                                        && x.EffectiveDate <= shippingDate
                                                        && x.ExpirationDate >= shippingDate)
                                           .FirstOrDefault();
            if (tariff == null)
            {
                return null;
            }

            decimal cost;
            if (shipping.TarifficationType == TarifficationType.Ftl && tariff.FtlRate != null)
            {
                cost = tariff.FtlRate.Value;
            }
            else
            {
                int totalPallets = orders.Sum(x => x.PalletsCount ?? 0);
                cost = GetLtlRate(tariff, totalPallets) * totalPallets ?? 0M;
            }

            bool needWinterCoeff = tariff.StartWinterPeriod != null
                                && tariff.EndWinterPeriod != null
                                && shippingDate >= tariff.StartWinterPeriod
                                && shippingDate <= tariff.EndWinterPeriod
                                && tariff.WinterAllowance != null;
            if (needWinterCoeff)
            {
                cost *= 1 + tariff.WinterAllowance.Value / 100;
            }

            return cost;
        }

        private decimal? GetLtlRate(Tariff tariff, int palletsCount)
        {
            if (palletsCount < 33)
            {
                string propertyName = nameof(tariff.LtlRate33).Replace("33", palletsCount.ToString());
                var property = tariff.GetType().GetProperty(propertyName);
                return (decimal?)property.GetValue(tariff);
            }
            else
            {
                return tariff.LtlRate33;
            }
        }
    }
}
