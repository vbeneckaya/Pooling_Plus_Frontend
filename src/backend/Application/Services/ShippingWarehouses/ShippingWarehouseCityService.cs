using DAL.Services;
using Domain.Persistables;
using Domain.Services.ShippingWarehouseCity;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Services.ShippingWarehouses
{
    public class ShippingWarehouseCityService: IShippingWarehouseCityService
    {
        private readonly ICommonDataService _dataService;

        public ShippingWarehouseCityService(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<LookUpDto> ForSelect()
        {
            return _dataService.GetDbSet<ShippingWarehouse>()
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
