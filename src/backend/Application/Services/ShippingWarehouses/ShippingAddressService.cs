using DAL.Services;
using Domain.Persistables;
using Domain.Services.ShippingWarehouses;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.ShippingWarehouses
{
    public class ShippingAddressService : IShippingAddressService
    {
        private readonly ICommonDataService _dataService;

        public ShippingAddressService(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<ShippingWarehouse>()
                                       .Where(x => !string.IsNullOrEmpty(x.Address))
                                       .OrderBy(x => x.Address)
                                       .ToList();

            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Address,
                    Value = entity.Id.ToString()
                };
            }
        }
    }
}
