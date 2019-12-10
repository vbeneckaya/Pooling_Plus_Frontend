using DAL.Services;
using Domain.Persistables;
using Domain.Services.Warehouses;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Warehouses
{
    public class ClientNameService : IClientNameService
    {
        public IEnumerable<LookUpDto> ForSelect()
        {
            var clientNames = _dataService.GetDbSet<Warehouse>()
                                          .Select(w => w.WarehouseName)
                                          .Distinct()
                                          .OrderBy(x => x)
                                          .ToList();
            foreach (var clientName in clientNames)
            {
                if (string.IsNullOrEmpty(clientName))
                {
                    continue;
                }
                var dto = new LookUpDto
                {
                    Value = clientName,
                    Name = clientName
                };
                yield return dto;
            }
        }

        public ClientNameService(ICommonDataService dataService)
        {
            _dataService = dataService;
        }

        private readonly ICommonDataService _dataService;
    }
}
