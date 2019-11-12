using DAL;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.WarehouseCity;
using Domain.Services.Warehouses;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.WarehouseCity
{
    public class WarehouseCityService : IWarehouseCityService
    {
        private readonly ICommonDataService _dataService;

        public WarehouseCityService(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<LookUpDto> ForSelect()
        {
            return _dataService.GetDbSet<Warehouse>()
                .Select(i => i.City)
                .Distinct()
                .Select(i => new LookUpDto
                { 
                    Value = i,
                    Name = i
                });
        }
    }
}
