using DAL;
using Domain.Services.PickingTypes;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.PickingTypes
{
    public class PickingTypesService : IPickingTypesService
    {
        public IEnumerable<LookUpDto> ForSelect()
        {
            var pickingTypes = db.Warehouses.Select(w => w.PickingType).ToList().Distinct().OrderBy(x => x);
            foreach (string pickingType in pickingTypes)
            {
                yield return new LookUpDto
                {
                    Name = pickingType,
                    Value = pickingType
                };
            }
        }

        public PickingTypesService(AppDbContext appDbContext)
        {
            db = appDbContext;
        }

        protected readonly AppDbContext db;
    }
}
