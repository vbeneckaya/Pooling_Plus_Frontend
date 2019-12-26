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

            var companyIds = dtos.Where(x => !string.IsNullOrEmpty(x.CompanyId?.Value))
                         .Select(x => x.CompanyId.Value.ToGuid())
                         .ToList();

            var companies = _dataService.GetDbSet<Company>()
                                           .Where(x => companyIds.Contains(x.Id))
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

                if (!string.IsNullOrEmpty(dto.CompanyId?.Value)
                    && companies.TryGetValue(dto.CompanyId.Value, out Company company))
                {
                    dto.CompanyId.Name = company.Name;
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
                CompanyId = entity.CompanyId == null ? null : new LookUpDto(entity.CompanyId.ToString()),
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
            entity.CompanyId = dto.CompanyId?.Value?.ToGuid();

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

            if (user?.CompanyId != null)
            {
                query = query.Where(i => i.CompanyId == user.CompanyId);
            }

            return query;
        }

        public override UserConfigurationDictionaryItem GetDictionaryConfiguration(Guid id)
        {
            var user = _userProvider.GetCurrentUser();
            var configuration = base.GetDictionaryConfiguration(id);

            var companyId = configuration.Columns.First(i => i.Name.ToLower() == nameof(User.CompanyId).ToLower());
            companyId.IsReadOnly = user.CompanyId != null;

            return configuration;
        }

        public override User FindByKey(UserDto dto)
        {
            return _dataService.GetDbSet<User>()
                .FirstOrDefault(i => i.Email == dto.Email);
        }
    }
}