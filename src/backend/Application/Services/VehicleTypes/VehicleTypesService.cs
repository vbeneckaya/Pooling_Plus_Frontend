using Application.Shared;
using Application.Shared.Excel;
using Application.Shared.Excel.Columns;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Services.VehicleTypes;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.VehicleTypes
{
    public class VehicleTypesService : DictonaryServiceBase<VehicleType, VehicleTypeDto>, IVehicleTypesService
    {
        public VehicleTypesService(ICommonDataService dataService, IUserProvider userProvider, IServiceProvider serviceProvider) 
            : base(dataService, userProvider, serviceProvider) 
        { }

        public override ValidateResult MapFromDtoToEntity(VehicleType entity, VehicleTypeDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);

            var validateResult = ValidateDto(dto);
            if (validateResult.IsError)
            {
                return validateResult;
            }

            entity.Name = dto.Name;
            entity.TonnageId = dto.TonnageId.ToGuid();
            entity.BodyTypeId = dto.BodyTypeId.ToGuid();
            entity.PalletsCount = dto.PalletsCount.ToInt();
            entity.IsActive = dto.IsActive.GetValueOrDefault(true);

            return new ValidateResult(null, entity.Id.ToString());
        }

        private ValidateResult ValidateDto(VehicleTypeDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidattionResult result = new DetailedValidattionResult();

            if (string.IsNullOrEmpty(dto.Name))
            {
                result.AddError(nameof(dto.Name), "vehicleTypes.emptyName".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            var existingRecord = this.FindByKey(dto);
            var hasDuplicates = existingRecord != null && existingRecord.Id != dto.Id.ToGuid();

            if (hasDuplicates)
            {
                result.AddError("duplicatedVehicleTypes", "vehicleTypes.duplicated".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        public override VehicleType FindByKey(VehicleTypeDto dto)
        {
            return _dataService.GetDbSet<VehicleType>()
                .FirstOrDefault(i => i.Name == dto.Name);
        }

        public override VehicleTypeDto MapFromEntityToDto(VehicleType entity)
        {
            return new VehicleTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                TonnageId = entity.TonnageId?.ToString(),
                BodyTypeId = entity.BodyTypeId?.ToString(),
                PalletsCount = entity.PalletsCount?.ToString(),
                IsActive = entity.IsActive
            };
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var vehicleTypes = _dataService.GetDbSet<VehicleType>()
                .Where(i => i.IsActive)
                .OrderBy(c => c.Name)
                .ToList();

            foreach (VehicleType vehicleType in vehicleTypes)
            {
                yield return new LookUpDto
                {
                    Name = vehicleType.Name,
                    Value = vehicleType.Id.ToString()
                };
            }
        }

        protected override IQueryable<VehicleType> ApplySort(IQueryable<VehicleType> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }

        protected override ExcelMapper<VehicleTypeDto> CreateExcelMapper()
        {
            return new ExcelMapper<VehicleTypeDto>(_dataService, _userProvider)
                .MapColumn(w => w.TonnageId, new DictionaryReferenceExcelColumn(GetTonnageIdByName, GetTonnageNameById))
                .MapColumn(w => w.BodyTypeId, new DictionaryReferenceExcelColumn(GetBodyTypeIdByName, GetBodyTypeNameById));
        }

        private Guid? GetTonnageIdByName(string name)
        {
            var entry = _dataService.GetDbSet<Tonnage>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private string GetTonnageNameById(Guid id)
        {
            var entry = _dataService.GetDbSet<Tonnage>().Find(id);
            return entry?.Name;
        }

        private Guid? GetBodyTypeIdByName(string name)
        {
            var entry = _dataService.GetDbSet<BodyType>().Where(t => t.Name == name).FirstOrDefault();
            return entry?.Id;
        }

        private string GetBodyTypeNameById(Guid id)
        {
            var entry = _dataService.GetDbSet<BodyType>().Find(id);
            return entry?.Name;
        }
    }
}
