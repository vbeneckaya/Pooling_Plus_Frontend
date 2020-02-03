using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Services.Shippings;

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
            var validState = new[] { ShippingState.ShippingCreated, ShippingState.ShippingRequestSent, ShippingState.ShippingRejectedByTc };
           
            if (shipping.Status == null
                || !validState.Contains(shipping.Status.Value)
                || shipping.CarrierId == null
                || shipping.TarifficationType == null)
            {
                return;
            }

            var orders = _commonDataService.GetDbSet<Order>()
                                           .Where(x => x.ShippingId == shipping.Id)
                                           .ToList()
                                           .Where(x => !string.IsNullOrEmpty(x.ShippingWarehouseId.ToString())
                                                    && !string.IsNullOrEmpty(x.DeliveryWarehouseId.ToString()))
                                           .ToList();

            Log.Information("Расчет стоимости перевозки запущен для {ShippingNumber}", shipping.ShippingNumber);

            decimal? totalDeliveryCost = 0;
            
            foreach (var group in orders.GroupBy(x => new { x.ShippingWarehouseId, x.DeliveryWarehouseId, x.ClientId }))
            {
                var hasIncompleteOrders = group.Any(x => x.DeliveryType != DeliveryType.Delivery
                                                         || x.PalletsCount == null
                                                         || x.PalletsCount <= 0
                                                         || x.ShippingDate == null
                                                         || x.DeliveryDate == null);
                if (hasIncompleteOrders)
                {
                    continue;
                }

                decimal? deliveryCost = GetOrderGroupDeliveryCost(group.Key.ShippingWarehouseId, group.Key.DeliveryWarehouseId, shipping, group.AsEnumerable());
                foreach (var order in group)
                {
                    if (!order.ManualDeliveryCost && order.DeliveryCost != deliveryCost)
                    {
                        _historyService.SaveImpersonated(null, order.Id, "fieldChanged",
                                                         nameof(order.DeliveryCost).ToLowerFirstLetter(),
                                                         order.DeliveryCost, deliveryCost);
                        order.DeliveryCost = deliveryCost;
                    }
                }

                totalDeliveryCost += deliveryCost ?? 0;
                
                shipping.TotalDeliveryCost = totalDeliveryCost;
            }
        }

        private decimal? GetOrderGroupDeliveryCost(Guid? shippingWarehouseId, Guid? deliveryWarehouseId, Shipping shipping, IEnumerable<Order> orders)
        {
            DateTime shippingDate = orders.Min(x => x.ShippingDate.Value);

            var tariff = _commonDataService.GetDbSet<Tariff>()
                .FirstOrDefault(x => x.CarrierId == shipping.CarrierId 
                                     && x.ProviderId == shipping.ProviderId 
                                     && x.VehicleTypeId == shipping.VehicleTypeId
//                                     && x.BodyTypeId == shipping.BodyTypeId
                                     && x.TarifficationType == shipping.TarifficationType
                                     && x.ShippingWarehouseId == shippingWarehouseId
                                     && x.DeliveryWarehouseId == deliveryWarehouseId
                                     && x.EffectiveDate <= shippingDate
                                     && x.ExpirationDate >= shippingDate);
            if (tariff == null)
            {
                tariff = _commonDataService.GetDbSet<Tariff>()
                        .FirstOrDefault(x => x.CarrierId == shipping.CarrierId
                                             && x.ProviderId == shipping.ProviderId 
                                             && x.VehicleTypeId == null
//                                             && x.BodyTypeId == null
                                             && x.TarifficationType == shipping.TarifficationType
                                             && x.ShippingWarehouseId == shippingWarehouseId
                                             && x.DeliveryWarehouseId == deliveryWarehouseId
                                             && x.EffectiveDate <= shippingDate
                                             && x.ExpirationDate >= shippingDate)
                    ;
            }

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
                int totalPallets = orders.Sum(x => x.ConfirmedPalletsCount ?? x.ActualPalletsCount ?? x.PalletsCount ?? 0);
                cost = GetLtlRate(tariff, totalPallets) ?? 0M;
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
