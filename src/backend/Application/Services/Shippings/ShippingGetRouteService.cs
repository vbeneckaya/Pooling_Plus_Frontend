using Domain.Persistables;
using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Services;
using Domain.Services.Shippings;
using ThinkingHome.Migrator.Framework.Extensions;

namespace Application.Services.Shippings
{
    public class ShippingGetRouteService : IShippingGetRouteService
    {
        private readonly ICommonDataService _dataService;

        public ShippingGetRouteService(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        public void UpdateRoute(Shipping shipping, IEnumerable<Order> orders)
        {
            shipping.Route = GetRoute(orders);
        }

        private string GetRoute(IEnumerable<Order> orders)
        {
            var shippingPoints = orders.Select(_ => new RoutePoint
                {Date = _.ShippingDate, WarehouseId = _.ShippingWarehouseId}).ToList();
            
            var deliveryPoints = orders.Select(_ => new RoutePoint
                {Date = _.DeliveryDate, WarehouseId = _.DeliveryWarehouseId}).ToList();

            var shippingWarehouseIds = shippingPoints.Where(x => x.WarehouseId.HasValue)
                .Select(x => x.WarehouseId.Value)
                .ToList();

            var shippingWarehouses = _dataService.GetDbSet<ShippingWarehouse>()
                .Where(x => shippingWarehouseIds.Contains(x.Id))
                .ToDictionary(x => x.Id);

            var deliveryWarehouseIds = deliveryPoints.Where(x => x.WarehouseId.HasValue)
                .Select(x => x.WarehouseId.Value)
                .ToList();

            var deliveryWarehouses = _dataService.GetDbSet<Warehouse>()
                .Where(x => deliveryWarehouseIds.Contains(x.Id))
                .ToDictionary(x => x.Id);

            foreach (var point in shippingPoints)
            {
                if (point.WarehouseId.HasValue &&
                    shippingWarehouses.TryGetValue(point.WarehouseId.Value, out ShippingWarehouse shippingWarehouse))
                {
                    point.WarehouseName = shippingWarehouse.WarehouseName;
                }
                else point.WarehouseName = "_";
            }

            foreach (var point in deliveryPoints)
            {
                if (point.WarehouseId.HasValue &&
                    deliveryWarehouses.TryGetValue(point.WarehouseId.Value, out Warehouse deliveryWarehouse))
                {
                    point.WarehouseName = deliveryWarehouse.WarehouseName;
                }
                else point.WarehouseName = "_";
            }

            var points = shippingPoints.Concat(deliveryPoints).OrderBy(_ => _.Date)
                .Select(_ => new {_.WarehouseId, _.WarehouseName})
                .Distinct().Select(_=>_.WarehouseName);
            
            return points.ToSeparatedString("-");
        }
    }

    public class RoutePoint
    {
        public DateTime? Date { get; set; }
        public Guid? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
}