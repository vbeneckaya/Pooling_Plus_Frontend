using DAL.Services;
using Domain.Persistables;
using Domain.Services.ShippingWarehouses;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.ShippingWarehouses
{
    public class ShippingWarehouseService: IShippingWarehouseService
    {
        private readonly ICommonDataService _dataService;

        public ShippingWarehouseService(ICommonDataService dataService)
        {
            _dataService = dataService;
        }
        
        public IEnumerable<LookUpDto> ForSelect()
        {
            return _dataService.GetDbSet<ShippingWarehouse>()
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
