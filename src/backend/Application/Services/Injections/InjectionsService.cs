using Application.Services.Triggers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.Injections;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Injections
{
    public class InjectionsService : DictonaryServiceBase<Injection, InjectionDto>, IInjectionsService
    {
        public InjectionsService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService) 
            : base(dataService, userProvider, triggersService) 
        { }

        public override ValidateResult MapFromDtoToEntity(Injection entity, InjectionDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Type = dto.Type;
            entity.FileName = dto.FileName;
            entity.Status = dto.Status;
            entity.ProcessTimeUtc = dto.ProcessTimeUtc;

            return new ValidateResult(null, entity.Id.ToString());
        }

        public override InjectionDto MapFromEntityToDto(Injection entity)
        {
            return new InjectionDto
            {
                Id = entity.Id.ToString(),
                Type = entity.Type,
                FileName = entity.FileName,
                Status = entity.Status,
                ProcessTimeUtc = entity.ProcessTimeUtc
            };
        }

        public IEnumerable<InjectionDto> GetByTaskName(string taskName)
        {
            var resultEntries = _dataService.GetDbSet<Injection>().Where(i => i.Type == taskName);
            var resultDtos = resultEntries.Select(MapFromEntityToDto).ToArray();
            return resultDtos;
        }
    }
}
