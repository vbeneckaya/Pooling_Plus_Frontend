using Application.Shared;
using DAL.Services;
using Domain.Persistables;
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
        public VehicleTypesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public override void MapFromDtoToEntity(VehicleType entity, VehicleTypeDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.Name = dto.Name;
        }

        public override VehicleTypeDto MapFromEntityToDto(VehicleType entity)
        {
            return new VehicleTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name
            };
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var vehicleTypes = _dataService.GetDbSet<VehicleType>().OrderBy(c => c.Name).ToList();
            foreach (VehicleType vehicleType in vehicleTypes)
            {
                yield return new LookUpDto
                {
                    Name = vehicleType.Name,
                    Value = vehicleType.Id.ToString()
                };
            }
        }
    }
}
