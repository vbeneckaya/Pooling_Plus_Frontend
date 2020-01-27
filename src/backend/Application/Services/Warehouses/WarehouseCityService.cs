using DAL.Services;
using Domain.Persistables;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;
using Domain.Services.Warehouses;

namespace Application.Services.WarehouseCity
{
    public class WarehouseService : IWarehouseService
    {
        private readonly ICommonDataService _dataService;

        public WarehouseService(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<LookUpDto> ForSelect()
        {
            return _dataService.GetDbSet<Warehouse>()
                .Select(i => i.City)
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Distinct()
                .Select(i => new LookUpDto
                { 
                    Value = i,
                    Name = i
                })
                .ToList();
        }
    }
}
