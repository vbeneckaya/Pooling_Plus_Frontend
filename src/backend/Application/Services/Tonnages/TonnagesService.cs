using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.Tonnages;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Services.Tonnages
{
    public class TonnagesService : DictonaryServiceBase<Tonnage, TonnageDto>, ITonnagesService
    {
        public TonnagesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public override ValidateResult MapFromDtoToEntity(Tonnage entity, TonnageDto dto)
        {
            entity.Name = dto.Name;

            return new ValidateResult(null, entity.Id.ToString());
        }

        public override TonnageDto MapFromEntityToDto(Tonnage entity)
        {
            return new TonnageDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name
            };
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<Tonnage>().OrderBy(x => x.Name).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Name,
                    Value = entity.Id.ToString()
                };
            }
        }

        protected override IQueryable<Tonnage> ApplySort(IQueryable<Tonnage> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }
    }
}
