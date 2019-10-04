using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.PickingTypes;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.PickingTypes
{
    public class PickingTypesService : DictonaryServiceBase<PickingType, PickingTypeDto>, IPickingTypesService
    {
        public override void MapFromDtoToEntity(PickingType entity, PickingTypeDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Name = dto.Name;
        }

        public override PickingTypeDto MapFromEntityToDto(PickingType entity)
        {
            return new PickingTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name
            };
        }

        public override DbSet<PickingType> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.PickingTypes;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var pickingTypes = db.PickingTypes.OrderBy(c => c.Name).ToList();
            foreach (PickingType pickingType in pickingTypes)
            {
                yield return new LookUpDto
                {
                    Name = pickingType.Name,
                    Value = pickingType.Id.ToString()
                };
            }
        }

        public PickingTypesService(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
