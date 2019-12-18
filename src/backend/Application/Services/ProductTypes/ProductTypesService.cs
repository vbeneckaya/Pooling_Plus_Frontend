using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.ProductTypes;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.ProductTypes
{
    public class ProductTypesService : DictionaryServiceBase<ProductType, ProductTypeDto>, IProductTypesService
    {
        public ProductTypesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService,
                                   IValidationService validationService, IFieldDispatcherService fieldDispatcherService, IFieldSetterFactory fieldSetterFactory)
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory)
        { }

        public override DetailedValidationResult MapFromDtoToEntity(ProductType entity, ProductTypeDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            entity.Name = dto.Name;
            entity.CompanyId = dto.CompanyId?.Value?.ToGuid();
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return null;
        }

        protected override DetailedValidationResult ValidateDto(ProductTypeDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<ProductType>()
                                            .Where(x => x.Name == dto.Name && x.Id.ToString() != dto.Id)
                                            .Any();

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "ProductType.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override ProductTypeDto MapFromEntityToDto(ProductType entity)
        {
            return new ProductTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive,
                CompanyId = entity.CompanyId == null ? null : new LookUpDto(entity.CompanyId.ToString()),
            };
        }

        protected override IEnumerable<ProductTypeDto> FillLookupNames(IEnumerable<ProductTypeDto> dtos)
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
            var productTypes = _dataService.GetDbSet<ProductType>()
                .Where(i => i.IsActive)
                .OrderBy(c => c.Name).ToList();

            foreach (ProductType productType in productTypes)
            {
                yield return new LookUpDto
                {
                    Name = productType.Name,
                    Value = productType.Id.ToString(),
                };
            }
        }

        protected override IQueryable<ProductType> ApplySort(IQueryable<ProductType> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }

        protected override ExcelMapper<ProductTypeDto> CreateExcelMapper()
        {
            return new ExcelMapper<ProductTypeDto>(_dataService, _userProvider, _fieldDispatcherService)
                .MapColumn(w => w.CompanyId, new DictionaryReferenceExcelColumn(GetCompanyIdByName));
        }

        public override ProductType FindByKey(ProductTypeDto dto)
        {
            return _dataService.GetDbSet<ProductType>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }

        private Guid? GetCompanyIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Company>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }
    }
}
