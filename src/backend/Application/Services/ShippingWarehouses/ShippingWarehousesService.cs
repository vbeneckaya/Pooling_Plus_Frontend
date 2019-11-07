using Application.Shared;
using DAL.Queries;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.ShippingWarehouses;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.ShippingWarehouses
{
    public class ShippingWarehousesService : DictonaryServiceBase<ShippingWarehouse, ShippingWarehouseDto>, IShippingWarehousesService
    {
        public ShippingWarehousesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider)
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
            if (!string.IsNullOrEmpty(dto.Id) && Guid.TryParse(dto.Id, out Guid id))
            {
                var dbSet = _dataService.GetDbSet<ShippingWarehouse>();
                return dbSet.GetById(id);
            }
            else
            {
                return _dataService.GetDbSet<ShippingWarehouse>().Where(x => x.Code == dto.Code).FirstOrDefault();
            }
        }

        public override void MapFromDtoToEntity(ShippingWarehouse entity, ShippingWarehouseDto dto)
        {
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
    }
}
