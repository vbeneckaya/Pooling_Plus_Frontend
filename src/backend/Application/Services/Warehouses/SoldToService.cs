using DAL;
using Domain.Persistables;
using Domain.Services.Warehouses;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Warehouses
{
    public class SoldToService : ISoldToService
    {
        public IEnumerable<SoldToDto> ForSelect()
        {
            var warehouses = _db.Warehouses.Where(w => w.SoldToNumber != null && w.SoldToNumber.Length > 0).OrderBy(w => w.SoldToNumber).ToList();
            foreach (Warehouse wh in warehouses)
            {
                SoldToDto dto = new SoldToDto
                {
                    Name = $"{wh.SoldToNumber} ({wh.WarehouseName})",
                    Value = wh.SoldToNumber,
                    WarehouseName = wh.WarehouseName,
                    Address = wh.Address,
                    City = wh.City,
                    Region = wh.Region,
                    LeadtimeDays = wh.LeadtimeDays,
                    PickingTypeId = wh.UsePickingType == "Да" ? wh.PickingTypeId?.ToString() : null
                };
                yield return dto;
            }
        }

        public SoldToService(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;
    }
}
