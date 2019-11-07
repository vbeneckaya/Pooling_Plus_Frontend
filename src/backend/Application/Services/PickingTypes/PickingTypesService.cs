﻿using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.PickingTypes;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.PickingTypes
{
    public class PickingTypesService : DictonaryServiceBase<PickingType, PickingTypeDto>, IPickingTypesService
    {
        public PickingTypesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public override ValidateResult MapFromDtoToEntity(PickingType entity, PickingTypeDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Name = dto.Name;

            return new ValidateResult(entity.Id.ToString());
        }

        public override PickingTypeDto MapFromEntityToDto(PickingType entity)
        {
            return new PickingTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name
            };
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var pickingTypes = _dataService.GetDbSet<PickingType>().OrderBy(c => c.Name).ToList();
            foreach (PickingType pickingType in pickingTypes)
            {
                yield return new LookUpDto
                {
                    Name = pickingType.Name,
                    Value = pickingType.Id.ToString()
                };
            }
        }

        protected override IQueryable<PickingType> ApplySort(IQueryable<PickingType> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }
    }
}
