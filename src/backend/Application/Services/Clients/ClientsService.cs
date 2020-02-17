﻿using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Services;
using Domain.Extensions;
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

namespace Application.Services.Clients
{
    public class ClientsService : DictionaryServiceBase<Client, ClientDto>, IClientsService
    {
        public ClientsService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService,
                              IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                              IFieldSetterFactory fieldSetterFactory, IAppConfigurationService appConfigurationService)
            : base(dataService, userProvider, triggersService, validationService, fieldDispatcherService, fieldSetterFactory, appConfigurationService)
        {
        }

        public override DetailedValidationResult MapFromDtoToEntity(Client entity, ClientDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            entity.Name = dto.Name;
            entity.Inn = dto.Inn;
            entity.Cpp = dto.Cpp;
            entity.LegalAddress = dto.LegalAddress;
            entity.ActualAddress = dto.ActualAddress;
            entity.ContactPerson = dto.ContactPerson;
            entity.ContactPhone = dto.ContactPhone;
            entity.Email = dto.Email;
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

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
            return new ClientDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                PoolingId = entity.PoolingId,
                Inn = entity.Inn,
                Cpp = entity.Cpp,
                LegalAddress = entity.LegalAddress,
                ActualAddress = entity.ActualAddress,
                ContactPerson = entity.ContactPerson,
                ContactPhone = entity.ContactPhone,
                Email = entity.Email,
                IsActive = entity.IsActive
            };
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
