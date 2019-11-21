using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using System;
using System.Linq;

namespace Application.Services.Shippings
{
    public class DeliveryCostCalcService : IDeliveryCostCalcService
    {
        private readonly ICommonDataService _commonDataService;

        public DeliveryCostCalcService(ICommonDataService commonDataService)
        {
            _commonDataService = commonDataService;
        }

        public decimal? CalculateDeliveryCost(Shipping shipping)
        {
            if (shipping.CarrierId == null
                || shipping.VehicleTypeId == null
                || shipping.BodyTypeId == null
                || shipping.TarifficationType == null
                || shipping.PalletsCount == null
                || shipping.PalletsCount == 0)
            {
                return null;
            }

            var orders = _commonDataService.GetDbSet<Order>()
                                           .Where(x => x.ShippingId == shipping.Id)
                                           .ToList();

            var hasIncompleteOrders = orders.Where(x => x.ShippingDate == null
                                                    || x.DeliveryDate == null
                                                    || string.IsNullOrEmpty(x.DeliveryCity))
                                            .Any();
            if (hasIncompleteOrders)
            {
                return null;
            }

            string deliveryCity = orders.Select(x => x.DeliveryCity).FirstOrDefault();
            DateTime shippingDate = orders.Min(x => x.ShippingDate.Value);

            var tariff = _commonDataService.GetDbSet<Tariff>()
                                           .Where(x => x.CarrierId == shipping.CarrierId
                                                        && x.VehicleTypeId == shipping.VehicleTypeId
                                                        && x.TarifficationType == shipping.TarifficationType
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
                cost = GetLtlRate(tariff, shipping.PalletsCount.Value) * shipping.PalletsCount ?? 0M;
            }

            bool needWinterCoeff = tariff.StartWinterPeriod != null
                                && tariff.EndWinterPeriod != null
                                && shippingDate >= tariff.StartWinterPeriod
                                && shippingDate <= tariff.EndWinterPeriod
                                && tariff.WinterAllowance != null;
            if (needWinterCoeff)
            {
                cost *= tariff.WinterAllowance.Value;
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
