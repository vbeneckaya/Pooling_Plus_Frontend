using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Injections;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Injections
{
    public class InjectionsService : DictonaryServiceBase<Injection, InjectionDto>, IInjectionsService
    {
        public override void MapFromDtoToEntity(Injection entity, InjectionDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Type = dto.Type;
            entity.FileName = dto.FileName;
            entity.Status = dto.Status;
            entity.ProcessTimeUtc = dto.ProcessTimeUtc;
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

        public override DbSet<Injection> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Injections;
        }

        public IEnumerable<InjectionDto> GetLast(string type, int hours)
        {
            DateTime barrier = DateTime.UtcNow.AddHours(-hours);
            var resultEntries = db.Injections.Where(i => i.Type == type && i.ProcessTimeUtc >= barrier);
            var resultDtos = resultEntries.Select(MapFromEntityToDto).ToArray();
            return resultDtos;
        }

        public InjectionsService(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
