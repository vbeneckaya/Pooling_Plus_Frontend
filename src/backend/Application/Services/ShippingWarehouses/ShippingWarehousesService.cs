﻿using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.ShippingWarehouses;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.ShippingWarehouses
{
    public class ShippingWarehousesService : DictonaryServiceBase<ShippingWarehouse, ShippingWarehouseDto>, IShippingWarehousesService
    {
        public ShippingWarehousesService(ICommonDataService dataService, IUserProvider userProvider, ILogger<ShippingWarehousesService> logger) : base(dataService, userProvider, logger)
        {
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

        public override ValidateResult MapFromDtoToEntity(ShippingWarehouse entity, ShippingWarehouseDto dto)
        {
            var validateResult = ValidateDto(dto);
            if (validateResult.IsError)
            {
                return validateResult;
            }

            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Code = dto.Code;
            entity.WarehouseName = dto.WarehouseName;
            entity.Address = dto.Address;
            entity.ValidAddress = dto.ValidAddress;
            entity.PostalCode = dto.PostalCode;
            entity.Region = dto.Region;
            entity.Area = dto.Area;
            entity.City = dto.City;
            entity.Street = dto.Street;
            entity.House = dto.House;
            entity.IsActive = dto.IsActive ?? false;

            return new ValidateResult(null, entity.Id.ToString());
        }

        public override ShippingWarehouseDto MapFromEntityToDto(ShippingWarehouse entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new ShippingWarehouseDto
            {
                Id = entity.Id.ToString(),
                Code = entity.Code,
                WarehouseName = entity.WarehouseName,
                Address = entity.Address,
                ValidAddress = entity.ValidAddress,
                PostalCode = entity.PostalCode,
                Region = entity.Region,
                Area = entity.Area,
                City = entity.City,
                Street = entity.Street,
                House = entity.House,
                IsActive = entity.IsActive
            };
        }

        protected override IQueryable<ShippingWarehouse> ApplySort(IQueryable<ShippingWarehouse> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.WarehouseName)
                .ThenBy(i => i.Id);
        }

        private ValidateResult ValidateDto(ShippingWarehouseDto dto)
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(dto.Code))
            {
                errors.Add("emptyCode".Translate(lang));
            }

            if (string.IsNullOrEmpty(dto.WarehouseName))
            {
                errors.Add("emptyWarehouseName".Translate(lang));
            }

            var hasDuplicates = _dataService.GetDbSet<ShippingWarehouse>()
                                            .Where(x => x.Code == dto.Code && x.Id.ToString() != dto.Id)
                                            .Any();
            if (hasDuplicates)
            {
                errors.Add("duplicateWarehouseCode".Translate(lang));
            }

            return new ValidateResult(string.Join(' ', errors));
        }
    }
}
