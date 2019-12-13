using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Queries;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Services.Users;
using Domain.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application.Services.Users
{
    public class UsersService : DictonaryServiceBase<User, UserDto>, IUsersService
    {
        private readonly IConfiguration _configuration;

        public UsersService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                            IValidationService validationService, IFieldDispatcherService fieldDispatcherService, IFieldSetterFactory fieldSetterFactory, IConfiguration configuration) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory) 
        {
            _configuration = configuration;
        }

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
                CarrierId = entity.CarrierId == null ? null : new LookUpDto(entity.CarrierId.ToString())
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

            var passwordValidation = ValidatePassword(dto.Password, lang);

            if (passwordValidation != null)
            {
                result.AddError(passwordValidation);
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

        private ValidationResultItem ValidatePassword(string value, string lang)
        {
            if (string.IsNullOrEmpty(value)) return null;

            List<string> errorMessages = new List<string>();

            var passwordConfig = _configuration.GetSection("PasswordRules");

            var passwordMinLength = passwordConfig["MinLength"].ToInt();

            if (passwordMinLength.HasValue && value.Length < passwordMinLength)
            {
                errorMessages.Add("User.Password.MinLength");
            }

            var validCharactersMatch = Regex.IsMatch(value, @"^[A-Za-z\d@$!%*?&]*$");

            if (!validCharactersMatch)
            {
                errorMessages.Add("User.Password.ValidCharacters");
            }

            var strongMatch = Regex.IsMatch(value, @"\d+");

            if (!strongMatch)
            {
                errorMessages.Add("User.Password.StrongPassword");
            }

            if (!errorMessages.Any()) return null;

            var message = string.Join(". ", errorMessages.Select(i => i.Translate(lang)));

            return new ValidationResultItem
            {
                Name = nameof(UserDto.Password).ToLowerFirstLetter(),
                Message = message,
                ResultType = ValidationErrorType.InvalidPassword
            };
        }

        protected override IQueryable<User> ApplySort(IQueryable<User> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Email)
                .ThenBy(i => i.Id);
        }

        public override User FindByKey(UserDto dto)
        {
            return _dataService.GetDbSet<User>()
                .FirstOrDefault(i => i.Email == dto.Email);
        }
    }
}