using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.DocumentTypes;
using Domain.Services.FieldProperties;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.DocumentTypes
{

    public class DocumentTypesService : DictionaryServiceBase<DocumentType, DocumentTypeDto>, IDocumentTypesService
    {
        public DocumentTypesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                                    IValidationService validationService, IFieldDispatcherService fieldDispatcherService, IFieldSetterFactory fieldSetterFactory) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory) 
        { }

        public override DetailedValidationResult MapFromDtoToEntity(DocumentType entity, DocumentTypeDto dto)
        {
            entity.Name = dto.Name;
            entity.CompanyId = dto.CompanyId?.Value?.ToGuid();
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return null;
        }
        protected override DetailedValidationResult ValidateDto(DocumentTypeDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<DocumentType>()
                                            .Where(x => x.Name == dto.Name && x.Id.ToString() != dto.Id)
                                            .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "DocumentType.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        protected override IEnumerable<DocumentTypeDto> FillLookupNames(IEnumerable<DocumentTypeDto> dtos)
        {
            var companyIds = dtos.Where(x => !string.IsNullOrEmpty(x.CompanyId?.Value))
             .Select(x => x.CompanyId.Value.ToGuid())
             .ToList();

            var companies = _dataService.GetDbSet<Company>()
                                           .Where(x => companyIds.Contains(x.Id))
                                           .ToDictionary(x => x.Id.ToString());

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.CompanyId?.Value)
                    && companies.TryGetValue(dto.CompanyId.Value, out Company company))
                {
                    dto.CompanyId.Name = company.Name;
                }

                yield return dto;
            }
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<DocumentType>()
                .Where(i => i.IsActive)
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

        public override DocumentTypeDto MapFromEntityToDto(DocumentType entity)
        {
            return new DocumentTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                CompanyId = entity.CompanyId == null ? null : new LookUpDto(entity.CompanyId.ToString()),
                IsActive = entity.IsActive
            };
        }

        protected override IQueryable<DocumentType> ApplySort(IQueryable<DocumentType> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }

        public override DocumentType FindByKey(DocumentTypeDto dto)
        {
            return _dataService.GetDbSet<DocumentType>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }
    }
}
