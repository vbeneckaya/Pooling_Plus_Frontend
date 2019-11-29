using Application.BusinessModels.ShippingWarehouses.Handlers;
using Application.Services.Addresses;
using Application.Services.Triggers;
using Application.Shared;
using AutoMapper;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Services.ShippingWarehouses;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.ShippingWarehouses
{
    public class ShippingWarehousesService : DictonaryServiceBase<ShippingWarehouse, ShippingWarehouseDto>, IShippingWarehousesService
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _historyService;
        private readonly ICleanAddressService _cleanAddressService;

        public ShippingWarehousesService(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                                         IHistoryService historyService, ICleanAddressService cleanAddressService) 
            : base(dataService, userProvider, triggersService)
        {
            _mapper = ConfigureMapper().CreateMapper();
            _historyService = historyService;
            _cleanAddressService = cleanAddressService;
        }

        public ShippingWarehouse GetByCode(string code)
        {
            return _dataService.GetDbSet<ShippingWarehouse>().FirstOrDefault(x => x.Code == code && x.IsActive);
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<ShippingWarehouse>().OrderBy(x => x.WarehouseName).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.WarehouseName,
                    Value = entity.Id.ToString()
                };
            }
        }

        public override ShippingWarehouse FindByKey(ShippingWarehouseDto dto)
        {
            return _dataService.GetDbSet<ShippingWarehouse>().Where(x => x.Code == dto.Code).FirstOrDefault();
        }

        public override DetailedValidationResult MapFromDtoToEntity(ShippingWarehouse entity, ShippingWarehouseDto dto)
        {
            var validateResult = ValidateDto(dto);
            if (validateResult.IsError)
            {
                return validateResult;
            }

            var setter = new FieldSetter<ShippingWarehouse>(entity, _historyService);

            if (!string.IsNullOrEmpty(dto.Id))
                setter.UpdateField(e => e.Id, Guid.Parse(dto.Id), ignoreChanges: true);
            setter.UpdateField(e => e.Code, dto.Code);
            setter.UpdateField(e => e.WarehouseName, dto.WarehouseName, new WarehouseNameHandler(_dataService, _historyService));
            setter.UpdateField(e => e.Address, dto.Address, new AddressHandler(_dataService, _historyService, _cleanAddressService));
            setter.UpdateField(e => e.ValidAddress, dto.ValidAddress, ignoreChanges: true);
            setter.UpdateField(e => e.PostalCode, dto.PostalCode, ignoreChanges: true);
            setter.UpdateField(e => e.Region, dto.Region);
            setter.UpdateField(e => e.Area, dto.Area, ignoreChanges: true);
            setter.UpdateField(e => e.City, dto.City, new CityHandler(_dataService, _historyService));
            setter.UpdateField(e => e.Street, dto.Street, ignoreChanges: true);
            setter.UpdateField(e => e.House, dto.House, ignoreChanges: true);
            setter.UpdateField(e => e.IsActive, dto.IsActive ?? true, ignoreChanges: true);

            setter.ApplyAfterActions();
            setter.SaveHistoryLog();

            string errors = setter.ValidationErrors;
            return new DetailedValidationResult(errors, entity.Id.ToString());
        }

        public override ShippingWarehouseDto MapFromEntityToDto(ShippingWarehouse entity)
        {
            if (entity == null)
            {
                return null;
            }
            return _mapper.Map<ShippingWarehouseDto>(entity);
        }

        protected override IQueryable<ShippingWarehouse> ApplySort(IQueryable<ShippingWarehouse> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.WarehouseName)
                .ThenBy(i => i.Id);
        }

        private DetailedValidationResult ValidateDto(ShippingWarehouseDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            DetailedValidationResult result = new DetailedValidationResult();

            if (string.IsNullOrEmpty(dto.Code))
            {
                result.AddError(nameof(dto.Code), "emptyCode".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            if (string.IsNullOrEmpty(dto.WarehouseName))
            {
                result.AddError(nameof(dto.WarehouseName), "emptyWarehouseName".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            var hasDuplicates = _dataService.GetDbSet<ShippingWarehouse>()
                                            .Where(x => x.Code == dto.Code && x.Id.ToString() != dto.Id)
                                            .Any();
            if (hasDuplicates)
            {
                result.AddError(nameof(dto.Code), "duplicateWarehouseCode".Translate(lang), ValidationErrorType.DuplicatedRecord);
            }

            return result;
        }

        private MapperConfiguration ConfigureMapper()
        {
            var result = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ShippingWarehouse, ShippingWarehouseDto>()
                    .ForMember(t => t.Id, e => e.MapFrom((s, t) => s.Id.ToString()));
            });
            return result;
        }
    }
}
