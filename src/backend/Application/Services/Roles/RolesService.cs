using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Roles;
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