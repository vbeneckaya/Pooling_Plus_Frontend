using System;
using System.Collections.Generic;
using System.Linq;
using Application.Shared;
using DAL.Queries;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Permissions;
using Domain.Services.Roles;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;

namespace Application.Services.Roles
{
    public class RolesService : DictonaryServiceBase<Role, RoleDto>, IRolesService
    {
        public RolesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public ValidateResult SetActive(Guid id, bool active)
        {
            var user = _userProvider.GetCurrentUser();
            var entity = _dataService.GetDbSet<Role>().GetById(id);
            if (entity == null)
            {
                return new ValidateResult("roleNotFound".translate(user.Language));
            }
            else
            {
                entity.IsActive = active;

                if (!entity.IsActive)
                {
                    _dataService.GetDbSet<User>()
                        .Where(i => i.RoleId == entity.Id)
                        .ToList()
                        .ForEach(i => i.IsActive = entity.IsActive);
                }

                _dataService.SaveChanges();

                return new ValidateResult(null, entity.Id.ToString());
            }
        }

        public ValidateResult SetPermissions(Guid roleId, IEnumerable<RolePermissions> permissions)
        {
            var user = _userProvider.GetCurrentUser();

            var entity = _dataService.GetDbSet<Role>().GetById(roleId);
            if (entity == null)
            {
                return new ValidateResult("roleNotFound".translate(user.Language));
            }

            entity.Permissions = permissions.Cast<int>().ToArray();

            return new ValidateResult(null, entity.Id.ToString());
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<Role>().Where(x => x.IsActive).OrderBy(x => x.Name).ToList();
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
            entity.Permissions = dto?.Permissions?.Select(i => i.Code)?.Cast<int>()?.ToArray();
        }

        public override RoleDto MapFromEntityToDto(Role entity)
        {
            return new RoleDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive,
                Permissions = entity?.Permissions?.Cast<RolePermissions>()?.Select(i => new PermissionInfo
                {
                    Code = i,
                    Name = i.GetPermissionName()
                }),
                UsersCount = _dataService.GetDbSet<User>().Where(i => i.RoleId == entity.Id).Count()
            };
        }

        public IEnumerable<PermissionInfo> GetAllPermissions()
        {
            return Enum.GetValues(typeof(RolePermissions))
                .Cast<RolePermissions>()
                .Select(i => new PermissionInfo
                {
                    Code = i,
                    Name = i.GetPermissionName()
                });
        }
    }
}