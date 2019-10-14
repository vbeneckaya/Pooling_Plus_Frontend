using System;
using System.Collections.Generic;
using System.Linq;
using Application.Shared;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using Domain.Services.Roles;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Roles
{
    public class RolesService : DictonaryServiceBase<Role, RoleDto>, IRolesService
    {
        public RolesService(AppDbContext appDbContext, IUserProvider userProvider) 
            : base(appDbContext)
        {
            _userProvider = userProvider;
        }

        public override DbSet<Role> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Roles;
        }

        public ValidateResult SetActive(Guid id, bool active)
        {
            var user = _userProvider.GetCurrentUser();
            var entity = db.Roles.GetById(id);
            if (entity == null)
            {
                return new ValidateResult("roleNotFound".translate(user.Language));
            }
            else
            {
                entity.IsActive = active;
                db.SaveChanges();

                return new ValidateResult(null, entity.Id.ToString());
            }
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = db.Roles.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();
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
            entity.IsActive = dto.IsActive;
        }

        public override RoleDto MapFromEntityToDto(Role entity)
        {
            return new RoleDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive
            };
        }

        private readonly IUserProvider _userProvider;
    }
}