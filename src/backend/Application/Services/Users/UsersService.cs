using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Queries;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.FieldProperties;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Services.Users;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Users
{
    public class UsersService : DictionaryServiceBase<User, UserDto>, IUsersService
    {
        public UsersService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                            IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                            IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, configurationService) 
        { }

        public ValidateResult SetActive(Guid id, bool active)
        {
            var user = _userProvider.GetCurrentUser();
            var entity = _dataService.GetDbSet<User>().GetById(id);
            if (entity == null)
            {
                return new ValidateResult("userNotFoundEntity".Translate(user.Language));
            }
            else
            {
                entity.IsActive = active;
                _dataService.SaveChanges();

                return new ValidateResult(null, entity.Id.ToString());
            }
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<User>()
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

        protected override IEnumerable<UserDto> FillLookupNames(IEnumerable<UserDto> dtos)
        {
            var carrierIds = dtos.Where(x => !string.IsNullOrEmpty(x.CarrierId?.Value))
                                 .Select(x => x.CarrierId.Value.ToGuid())
                                 .ToList();
            var carriers = _dataService.GetDbSet<TransportCompany>()
                                       .Where(x => carrierIds.Contains(x.Id))
                                       .ToDictionary(x => x.Id.ToString());

            var roleIds = dtos.Where(x => !string.IsNullOrEmpty(x.RoleId?.Value))
                              .Select(x => x.RoleId.Value.ToGuid())
                              .ToList();
            var roles = _dataService.GetDbSet<Role>()
                                    .Where(x => roleIds.Contains(x.Id))
                                    .ToDictionary(x => x.Id.ToString());

            var clientIds = dtos.Where(x => !string.IsNullOrEmpty(x.ClientId?.Value))
                         .Select(x => x.ClientId.Value.ToGuid())
                         .ToList();

            var clients = _dataService.GetDbSet<Client>()
                                           .Where(x => clientIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());
            
            var providerIds = dtos.Where(x => !string.IsNullOrEmpty(x.ProviderId?.Value))
                .Select(x => x.ProviderId.Value.ToGuid())
                .ToList();

            var providers = _dataService.GetDbSet<Provider>()
                .Where(x => providerIds.Contains(x.Id))
                .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.CarrierId?.Value)
                    && carriers.TryGetValue(dto.CarrierId.Value, out TransportCompany carrier))
                {
                    dto.CarrierId.Name = carrier.Title;
                }

                if (!string.IsNullOrEmpty(dto.RoleId?.Value)
                    && roles.TryGetValue(dto.RoleId.Value, out Role role))
                {
                    dto.RoleId.Name = role.Name;
                }

                if (!string.IsNullOrEmpty(dto.ClientId?.Value)
                    && clients.TryGetValue(dto.ClientId.Value, out Client client))
                {
                    dto.ClientId.Name = client.Name;
                }
                
                if (!string.IsNullOrEmpty(dto.ProviderId?.Value)
                    && providers.TryGetValue(dto.ProviderId.Value, out Provider provider))
                {
                    dto.ProviderId.Name = provider.Name;
                }

                yield return dto;
            }
        }

        public override UserDto MapFromEntityToDto(User entity)
        {
            var roles = _dataService.GetDbSet<Role>().ToList();
            return new UserDto
            {
                Email = entity.Email,
                Id = entity.Id.ToString(),
                UserName = entity.Name,
                Role = roles.FirstOrDefault(role => role.Id == entity.RoleId).Name,
                RoleId = new LookUpDto(entity.RoleId.ToString()),
                FieldsConfig = entity.FieldsConfig,
                IsActive = entity.IsActive,
                CarrierId = entity.CarrierId == null ? null : new LookUpDto(entity.CarrierId.ToString()),
                ClientId = entity.ClientId == null ? null : new LookUpDto(entity.ClientId.ToString()),
                ProviderId = entity.ProviderId == null ? null : new LookUpDto(entity.ProviderId.ToString()),
            };
        }

        public override DetailedValidationResult MapFromDtoToEntity(User entity, UserDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id)) 
                entity.Id = Guid.Parse(dto.Id);

            var oldRoleId = entity.RoleId;

            entity.Email = dto.Email;
            entity.Name = dto.UserName;
            entity.RoleId = Guid.Parse(dto.RoleId?.Value);
            entity.FieldsConfig = dto.FieldsConfig;
            entity.IsActive = dto.IsActive;
            entity.CarrierId = dto.CarrierId?.Value?.ToGuid();
            entity.ClientId = dto.ClientId?.Value?.ToGuid();
            entity.ProviderId = dto.ProviderId?.Value?.ToGuid();

            var transportCompanyRole = _dataService.GetDbSet<Role>().First(i => i.Name == "TransportCompanyEmployee");

            if (oldRoleId != entity.RoleId && entity.RoleId != transportCompanyRole.Id)
            {
                entity.CarrierId = null;
            }

            
            if (!string.IsNullOrEmpty(dto.Password)) 
                entity.PasswordHash = dto.Password.GetHash();

            return null;
        }

        protected override DetailedValidationResult ValidateDto(UserDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            if (string.IsNullOrEmpty(dto.Id) && string.IsNullOrEmpty(dto.Password))
            {
                result.AddError(nameof(dto.Password), "User.Password.ValueIsRequired".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            var hasDuplicates = this._dataService.GetDbSet<User>()
                .Where(i => i.Id != dto.Id.ToGuid())
                .Where(i => i.Email == dto.Email)
                .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Email), "User.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        protected override IQueryable<User> ApplySort(IQueryable<User> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Email)
                .ThenBy(i => i.Id);
        }
        public override IQueryable<User> ApplyRestrictions(IQueryable<User> query)
        {
            var currentUserId = _userProvider.GetCurrentUserId();
            var user = _dataService.GetById<User>(currentUserId.Value);

            // Local user restrictions

            if (user?.ClientId != null)
            {
                query = query.Where(i => i.ClientId == user.ClientId);
            }
            
            if (user?.ProviderId != null)
            {
                query = query.Where(i => i.ProviderId == user.ProviderId);
            }
            
            if (user?.CarrierId != null)
            {
                query = query.Where(i => i.CarrierId == user.CarrierId);
            }

            return query;
        }

        public override User FindByKey(UserDto dto)
        {
            return _dataService.GetDbSet<User>()
                .FirstOrDefault(i => i.Email == dto.Email);
        }
    }
}