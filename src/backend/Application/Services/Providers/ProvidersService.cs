using System;
using System.Collections.Generic;
using System.Linq;
using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using AutoMapper;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.FieldProperties;
using Domain.Services.Providers;
using Domain.Services.ShippingWarehouses;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;

namespace Application.Services.Providers
{
    public class ProvidersService : DictionaryServiceBase<Provider, ProviderDto>, IProvidersService
    {
        private readonly IMapper _mapper;

        private readonly IShippingWarehousesService _shippingWarehousesService;
        
        public ProvidersService(ICommonDataService dataService, IUserProvider userProvider,
            ITriggersService triggersService,
            IValidationService validationService, IFieldDispatcherService fieldDispatcherService,
            IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService,
            IShippingWarehousesService shippingWarehousesService)
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService,
                fieldSetterFactory, configurationService)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _shippingWarehousesService = shippingWarehousesService;
        }
        
        
    
        public override DetailedValidationResult MapFromDtoToEntity(Provider entity, ProviderDto dto)
        {
            _mapper.Map(dto, entity);
            return null;
        }

        public override ProviderDto MapFromEntityToDto(Provider entity)
        {
            return _mapper.Map<Provider, ProviderDto>(entity); 
        }
        protected override DetailedValidationResult ValidateDto(ProviderDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<Provider>()
                                .Where(x => x.Name == dto.Name && x.Id.ToString() != dto.Id)
                                .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "Provider.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }
        
        public override DetailedValidationResult SaveOrCreate(ProviderDto entityFrom)
        {
            var isNew = string.IsNullOrEmpty(entityFrom.Id);
           
            var result =  SaveOrCreateInner(entityFrom, false);
            
            if (isNew && !result.IsError)
            {
                _shippingWarehousesService.AddColedinoShippingWarehouseToProvider(Guid.Parse(result.Id));
            }

            return result;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<Provider>()
                .Where(i => i.IsActive)
                .OrderBy(x => x.Name)
                .ToList();

            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Name,
                    Value = entity.Id.ToString(),
                };
            }
        }

        protected override IQueryable<Provider> ApplySort(IQueryable<Provider> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }

        public override Provider FindByKey(ProviderDto dto)
        {
            return _dataService.GetDbSet<Provider>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }
        
        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Provider, ProviderDto>()
                    .ForMember(t=>t.Id, e=>e.MapFrom(s=>s.Id.ToString()));
                
                cfg.CreateMap<ProviderDto, Provider>()
                    .ForMember(t=>t.Id, e=>e.MapFrom(s=> string.IsNullOrEmpty(s.Id) ? Guid.Empty : Guid.Parse(s.Id)));
            });
            return result;
        }
    }
}
