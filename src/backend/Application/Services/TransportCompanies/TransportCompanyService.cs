using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.FieldProperties;
using Domain.Services.Translations;
using Domain.Services.TransportCompanies;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Application.Services.TransportCompanies
{
    public class TransportCompaniesService : DictionaryServiceBase<TransportCompany, TransportCompanyDto>,
        ITransportCompaniesService
    {
        private readonly IMapper _mapper;
        
        public TransportCompaniesService(ICommonDataService dataService, IUserProvider userProvider,
            ITriggersService triggersService,
            IValidationService validationService, IFieldDispatcherService fieldDispatcherService,
            IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService)
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService,
                fieldSetterFactory, configurationService)
        {
            _mapper = ConfigureMapper().CreateMapper();
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            var carriers = _dataService.GetDbSet<TransportCompany>()
                .Where(i => i.IsActive)
                .OrderBy(c => c.Title)
                .ToList();

            var empty = new LookUpDto
            {
                Name = "emptyValue".Translate(lang),
                Value = LookUpDto.EmptyValue,
                IsFilterOnly = true
            };
            yield return empty;

            foreach (TransportCompany carrier in carriers)
            {
                yield return new LookUpDto
                {
                    Name = carrier.Title,
                    Value = carrier.Id.ToString()
                };
            }
        }

        public override DetailedValidationResult MapFromDtoToEntity(TransportCompany entity, TransportCompanyDto dto)
        {
            _mapper.Map(dto, entity);

            return null;
        }

        protected override DetailedValidationResult ValidateDto(TransportCompanyDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = _dataService.GetDbSet<TransportCompany>()
                .Where(x => !string.IsNullOrEmpty(dto.Title) && x.Title.ToLower() == dto.Title.ToLower())
                .Any(x => x.Id.ToString() != dto.Id);

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Title), "TransportCompany.DuplicatedRecord".Translate(lang),
                    ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override TransportCompanyDto MapFromEntityToDto(TransportCompany entity)
        {
            return _mapper.Map<TransportCompany, TransportCompanyDto>(entity); 
        }

        protected override IQueryable<TransportCompany> ApplySort(IQueryable<TransportCompany> query,
            SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Title)
                .ThenBy(i => i.Id);
        }

        public override TransportCompany FindByKey(TransportCompanyDto dto)
        {
            return _dataService.GetDbSet<TransportCompany>()
                .FirstOrDefault(i => i.Title == dto.Title);
        }
        
        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TransportCompany, TransportCompanyDto>()
                    .ForMember(t=>t.Id, e=>e.MapFrom(s=>s.Id.ToString()));
                
                cfg.CreateMap<TransportCompanyDto, TransportCompany>()
                    .ForMember(t=>t.Id, e=>e.MapFrom(s=> string.IsNullOrEmpty(s.Id) ? Guid.Empty : Guid.Parse(s.Id)));
            });
            return result;
        }

//        protected override ExcelMapper<TransportCompanyDto> CreateExcelMapper()
//        {
//            return base.CreateExcelMapper()
//                .MapColumn(w => w.CompanyId, new DictionaryReferenceExcelColumn(GetCompanyIdByName));
//        }
    }
}