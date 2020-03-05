using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.Clients;
using Domain.Services.FieldProperties;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Application.Services.Clients
{
    public class ClientsService : DictionaryServiceBase<Client, ClientDto>, IClientsService
    {
        private readonly IMapper _mapper;
        
        public ClientsService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService,
                              IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                              IFieldSetterFactory fieldSetterFactory, IAppConfigurationService appConfigurationService)
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, appConfigurationService)
        {
            _mapper = ConfigureMapper().CreateMapper();
        }

        public override DetailedValidationResult MapFromDtoToEntity(Client entity, ClientDto dto)
        {
            _mapper.Map(dto, entity);

            return null;
        }

        protected override DetailedValidationResult ValidateDto(ClientDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<Client>()
                                .Where(x => x.Name == dto.Name && x.Id.ToString() != dto.Id)
                                .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "Client.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override Client FindByKey(ClientDto dto)
        {
            return _dataService.GetDbSet<Client>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }

        public override ClientDto MapFromEntityToDto(Client entity)
        {
            return _mapper.Map<Client, ClientDto>(entity); 
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            var vehicleTypes = _dataService.GetDbSet<Client>()
                .Where(i => i.IsActive)
                .OrderBy(c => c.Name)
                .ToList();

            var empty = new LookUpDto
            {
                Name = "emptyValue".Translate(lang),
                Value = LookUpDto.EmptyValue,
                IsFilterOnly = true
            };
            yield return empty;

            foreach (Client vehicleType in vehicleTypes)
            {
                yield return new LookUpDto
                {
                    Name = vehicleType.Name,
                    Value = vehicleType.Id.ToString()
                };
            }
        }
        
//        public override IQueryable<Client> ApplyRestrictions(IQueryable<Client> query)
//        {
//            var currentUserId = _userProvider.GetCurrentUserId();
//            var user = _dataService.GetById<User>(currentUserId.Value);
//
//            // Local user restrictions
//
//            if (user?.CompanyId != null)
//            {
//                query = query.Where(i => i.CompanyId == user.CompanyId);
//            }
//
//            return query;
//        }

        protected override IQueryable<Client> ApplySort(IQueryable<Client> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }
        
        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Client, ClientDto>()
                    .ForMember(t=>t.Id, e=>e.MapFrom(s=>s.Id.ToString()));
                
                cfg.CreateMap<ClientDto, Client>()
                    .ForMember(t=>t.Id, e=>e.MapFrom(s=> string.IsNullOrEmpty(s.Id) ? Guid.Empty : Guid.Parse(s.Id)));
            });
            return result;
        }

//        protected override ExcelMapper<ClientDto> CreateExcelMapper()
//        {
//            return new ExcelMapper<ClientDto>(_dataService, _userProvider, _fieldDispatcherService)
//                .MapColumn(w => w.CompanyId, new DictionaryReferenceExcelColumn(GetCompanyIdByName));
//        }

//        private Guid? GetCompanyIdByName(string name)
//        {
//            var entry = _dataService.GetDbSet<Company>().Where(t => t.Name == name).FirstOrDefault();
//            return entry?.Id;
//        }
    }
}
