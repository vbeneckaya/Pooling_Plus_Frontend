using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain.Services.ShippingWarehouses;

namespace Application.Services.ShippingWarehouses
{
    public class ShippingWarehousesForOrderCreation : IShippingWarehousesForOrderCreation
    {
        public IEnumerable<ShippingWarehouseDtoForSelect> ForSelect()
        {
            var warehouses = _db.ShippingWarehouses.Where(x=>x.IsActive).OrderBy(w => w.WarehouseName).ToList();
            foreach (var wh in warehouses)
            {
                var dto = new ShippingWarehouseDtoForSelect
                {
                    Name = wh.WarehouseName,
                    Address = wh.Address,
                    Value = wh.Id.ToString(),
                };
                yield return dto;
            }
        }

        public ShippingWarehousesForOrderCreation(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}
