﻿using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.FieldProperties;
using Domain.Services.Tonnages;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.Tonnages
{
    public class TonnagesService : DictionaryServiceBase<Tonnage, TonnageDto>, ITonnagesService
    {
        public TonnagesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                               IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                               IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService) 
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, configurationService) 
        { }

        public override DetailedValidationResult MapFromDtoToEntity(Tonnage entity, TonnageDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return null;
        }

        public override TonnageDto MapFromEntityToDto(Tonnage entity)
        {
            return new TonnageDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                IsActive = entity.IsActive,
            };
        }

        protected override DetailedValidationResult ValidateDto(TonnageDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = base.ValidateDto(dto);

            var hasDuplicates = !result.IsError && _dataService.GetDbSet<Tonnage>()
                                            .Any(x => x.Name == dto.Name && x.Id.ToString() != dto.Id);

            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Name), "Tonnage.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override IEnumerable<LookUpDto> ForSelect(Guid? filter = null)
        {
            var entities = _dataService.GetDbSet<Tonnage>()
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

        protected override IQueryable<Tonnage> ApplySort(IQueryable<Tonnage> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }
       
        public override Tonnage FindByKey(TonnageDto dto)
        {
            return _dataService.GetDbSet<Tonnage>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }
    }
}
