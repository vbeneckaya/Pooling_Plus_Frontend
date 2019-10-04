using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.PickingTypes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/pickingTypes")]
    public class PickingTypesController : DictionaryController<IPickingTypesService, PickingType, PickingTypeDto>
    {
        public PickingTypesController(IPickingTypesService vehicleTypesService) : base(vehicleTypesService)
        {
        }
    }
}