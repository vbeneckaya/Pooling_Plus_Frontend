using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.BodyTypes;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Services.BodyTypes
{
    public class BodyTypesService : DictonaryServiceBase<BodyType, BodyTypeDto>, IBodyTypesService
    {
        public BodyTypesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public override ValidateResult MapFromDtoToEntity(BodyType entity, BodyTypeDto dto)
        {
            entity.Name = dto.Name;

            return new ValidateResult(null, entity.Id.ToString());
        }

        public override BodyTypeDto MapFromEntityToDto(BodyType entity)
        {
            return new BodyTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name
            };
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<BodyType>().OrderBy(x => x.Name).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Name,
                    Value = entity.Id.ToString()
                };
            }
        }

        protected override IQueryable<BodyType> ApplySort(IQueryable<BodyType> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }
    }
}
