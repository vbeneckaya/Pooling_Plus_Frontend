using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.VehicleTypes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/vehicleTypes")]
    public class VehicleTypesController : DictionaryController<IVehicleTypesService, VehicleType, VehicleTypeDto>
    {
        public VehicleTypesController(IVehicleTypesService vehicleTypesService) : base(vehicleTypesService)
        {
        }
    }
}
