using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Warehouses;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Warehouses
{
    public class DeliveryAddressService : IDeliveryAddressService
    {
        private readonly ICommonDataService _dataService;

        public DeliveryAddressService(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<LookUpDto> ForSelect(string clientId, string deliveryCity)
        {
            Guid? clientIdValue = clientId.ToGuid();
            var entities = _dataService.GetDbSet<Warehouse>()
                                       .Where(x => !string.IsNullOrEmpty(x.Address)
                                                && (clientIdValue == null || x.ClientId == clientIdValue)
                                                && (string.IsNullOrEmpty(deliveryCity) || x.City == deliveryCity))
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
