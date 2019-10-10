using System;
using System.Collections.Generic;
using System.Linq;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Roles;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Roles
{
    public class RolesService : DictonaryServiceBase<Role, RoleDto>, IRolesService
    {
        public RolesService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<Role> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Roles;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = db.Roles.OrderBy(x => x.Name).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Name,
                    Value = entity.Id.ToString()
                };
            }
        }

        public override void MapFromDtoToEntity(Role entity, RoleDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            
            entity.Name = dto.Name;
        }

        public override RoleDto MapFromEntityToDto(Role entity)
        {
            return new RoleDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name
            };
        }
    }
}