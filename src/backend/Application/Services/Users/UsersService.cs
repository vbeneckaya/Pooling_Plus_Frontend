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
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace Application.Services.Users
{
    public class UsersService : DictionaryServiceBase<User, UserDto>, IUsersService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsersService(
            ICommonDataService dataService, 
            IUserProvider userProvider,
            ITriggersService triggersService,
            IValidationService validationService, 
            IFieldDispatcherService fieldDispatcherService,
            IFieldSetterFactory fieldSetterFactory, 
            IAppConfigurationService configurationService,
            IConfiguration configuration
            )
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService,
                fieldSetterFactory, configurationService)
        {
            _mapper = ConfigureMapper().CreateMapper();
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

        public override IEnumerable<LookUpDto> ForSelect(Guid? filter = null)
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
            var userDto = new UserDto();
            _mapper.Map(entity, userDto);
            
            return userDto;
        }

        public override DetailedValidationResult MapFromDtoToEntity(User entity, UserDto dto)
        {
            _mapper.Map(dto, entity);
            
            var oldRoleId = entity.RoleId;
            if (oldRoleId != entity.RoleId)
            {
                var transportCompanyRoleIds = _dataService.GetDbSet<Role>()
                    .Where(i => i.RoleType == Domain.Enums.RoleTypes.TransportCompany)
                    .Select(_ => _.Id)
                    .ToArray();

                var clientRoleIds = _dataService.GetDbSet<Role>()
                    .Where(i => i.RoleType == Domain.Enums.RoleTypes.Client)
                    .Select(_ => _.Id)
                    .ToArray();

                var providerRoleIds = _dataService.GetDbSet<Role>()
                    .Where(i => i.RoleType == Domain.Enums.RoleTypes.Provider)
                    .Select(_ => _.Id)
                    .ToArray();

                if (transportCompanyRoleIds.Contains(entity.RoleId))
                {
                    entity.ProviderId = null;
                    entity.ClientId = null;
                }

                if (clientRoleIds.Contains(entity.RoleId))
                {
                    entity.ProviderId = null;
                    entity.CarrierId = null;
                }

                if (providerRoleIds.Contains(entity.RoleId))
                {
                    entity.CarrierId = null;
                    entity.ClientId = null;
                }
            }


            if (!string.IsNullOrEmpty(dto.Password))
            {
                entity.PasswordHash = dto.Password.GetHash();
            }

            return null;
        }

        protected override DetailedValidationResult ValidateDto(UserDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            if (string.IsNullOrEmpty(dto.Id) && string.IsNullOrEmpty(dto.Password))
            {
                result.AddError(nameof(dto.Password), "User.Password.ValueIsRequired".Translate(lang),
                    ValidationErrorType.ValueIsRequired);
            }

            var hasDuplicates = this._dataService.GetDbSet<User>()
                .Where(i => i.Id != dto.Id.ToGuid())
                .Where(i => i.Email == dto.Email)
                .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Email), "User.DuplicatedRecord".Translate(lang),
                    ValidationErrorType.DuplicatedRecord);
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

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDto, User>()
                    .ForMember(t => t.Id,
                        e => e.MapFrom(s => string.IsNullOrEmpty(s.Id) ? Guid.Empty : Guid.Parse(s.Id)))
                    .ForMember(t => t.Name, e => e.MapFrom(s => s.UserName))
                    .ForMember(t => t.RoleId, e => e.MapFrom(s => Guid.Parse(s.RoleId.Value)))
                    .ForMember(t => t.Role, e => e.Ignore())
                    
                    
                    .ForMember(t => t.CarrierId, e => e.Condition((s)=> s.CarrierId?.Value != null))
                    .ForMember(t => t.CarrierId, e => e.MapFrom((s)=> Guid.Parse(s.CarrierId.Value)))
                    
                    .ForMember(t => t.ProviderId, e => e.Condition((s)=> s.ProviderId?.Value != null))
                    .ForMember(t => t.ProviderId, e => e.MapFrom((s)=> Guid.Parse(s.ProviderId.Value)))
                    
                    .ForMember(t => t.ClientId, e => e.Condition((s)=> s.ClientId?.Value != null))
                    .ForMember(t => t.ClientId, e => e.MapFrom((s)=> Guid.Parse(s.ClientId.Value)))
                    
                    .ForMember(t => t.PasswordHash,  e => e.Condition((s) => !string.IsNullOrEmpty(s.Password)))
                    .ForMember(t => t.PasswordHash, e => e.MapFrom((s) => s.Password.GetHash()))
                    ;

                var roles = _dataService.GetDbSet<Role>().ToList();
                cfg.CreateMap<User, UserDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()))
                    .ForMember(t => t.UserName, e => e.MapFrom((s) => s.Name))
                    .ForMember(t => t.RoleId, e => e.MapFrom((s) => new LookUpDto(s.RoleId.ToString())))
                    .ForMember(t => t.Role, e=>e.MapFrom((s) =>  roles.FirstOrDefault(role => role.Id == s.RoleId).Name))
                    
                    .ForMember(t => t.SignWithoutLoginLink, e => e.MapFrom((s) => 
                        $"{_configuration.GetSection("PoolingPlusUrl").Value}{s.Id.ToString()}/{Uri.EscapeDataString(s.PasswordHash.GetHash())}"
                        ))
                    
                    .ForMember(t => t.CarrierId, e => e.MapFrom( s => s.CarrierId == null ? null : new LookUpDto(s.CarrierId.ToString())))
                    .ForMember(t => t.ProviderId, e => e.MapFrom((s) => s.ProviderId == null ? null : new LookUpDto(s.ProviderId.ToString())))
                    .ForMember(t => t.ClientId, e => e.MapFrom((s) => s.ClientId == null ? null : new LookUpDto(s.ClientId.ToString())))
                    ;
            });
            return result;
        }
    }
}