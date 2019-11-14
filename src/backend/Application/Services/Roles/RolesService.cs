using Application.BusinessModels.Shared.Actions;
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
using System;
using System.Collections.Generic;
using System.Linq;

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
                return new ValidateResult("roleNotFound".Translate(user.Language));
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

        protected override IQueryable<Role> ApplySort(IQueryable<Role> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
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

        public override ValidateResult MapFromDtoToEntity(Role entity, RoleDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            
            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive;
            entity.Actions = dto.Actions?.ToArray();
            entity.Permissions = dto?.Permissions?.Select(i => i.Code)?.Cast<int>()?.ToArray();

            return new ValidateResult(null, entity.Id.ToString());
        }

        public override RoleDto MapFromEntityToDto(Role entity)
        {
            return new RoleDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive,
                Actions = entity.Actions,
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
                .Where(x => x != RolePermissions.None)
                .Select(i => new PermissionInfo
                {
                    Code = i,
                    Name = i.GetPermissionName()
                });
        }

        public RoleActionsDto GetAllActions()
        {
            var result = new RoleActionsDto
            {
                OrderActions = GetActions<Order>(),
                ShippingActions = GetActions<Shipping>()
            };
            return result;
        }

        private IEnumerable<string> GetActions<TEntity>()
        {
            var actionType = typeof(IAction<TEntity>);
            var actions = AppDomain.CurrentDomain
                                   .GetAssemblies()
                                   .SelectMany(s => s.GetTypes())
                                   .Where(p => actionType.IsAssignableFrom(p));
            return actions.Select(x => x.Name.ToLowerFirstLetter());
        }
    }
}