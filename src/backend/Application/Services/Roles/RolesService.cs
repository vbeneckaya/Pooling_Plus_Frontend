using Application.BusinessModels.Shared.Actions;
using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Queries;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
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
    public class RolesService : DictionaryServiceBase<Role, RoleDto>, IRolesService
    {
        public RolesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                            IValidationService validationService, IFieldDispatcherService fieldDispatcherService, IFieldSetterFactory fieldSetterFactory) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory) 
        { }

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
            var entities = _dataService.GetDbSet<Role>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToList();

            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Name,
                    Value = entity.Id.ToString()
                };
            }
        }

        public override DetailedValidationResult MapFromDtoToEntity(Role entity, RoleDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            
            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive;
            entity.Actions = dto.Actions?.ToArray();
            entity.Permissions = dto?.Permissions?.Select(i => i.Code)?.Cast<int>()?.ToArray();

            return null;
        }

        protected override DetailedValidationResult ValidateDto(RoleDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && this._dataService.GetDbSet<Role>()
                .Where(i => i.Id != dto.Id.ToGuid())
                .Where(i => i.Name == dto.Name)
                .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "Role.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
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
            return Domain.Extensions.Extensions.GetOrderedEnum<RolePermissions>()
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
            var actionSingleType = typeof(IAction<TEntity>);
            var actionGroupType = typeof(IAction<IEnumerable<TEntity>>);
            var actions = AppDomain.CurrentDomain
                                   .GetAssemblies()
                                   .SelectMany(s => s.GetTypes())
                                   .Where(p => actionSingleType.IsAssignableFrom(p) || actionGroupType.IsAssignableFrom(p));
            return actions.Select(x => x.Name.ToLowerFirstLetter());
        }

        public override Role FindByKey(RoleDto dto)
        {
            return _dataService.GetDbSet<Role>()
                .Where(i => i.Name == dto.Name)
                .FirstOrDefault();
        }
    }
}