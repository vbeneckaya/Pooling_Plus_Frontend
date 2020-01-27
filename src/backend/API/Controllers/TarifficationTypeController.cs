using Domain.Enums;
using Domain.Services.TarifficationTypes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/tarifficationType")]
    public class TarifficationTypeController : EnumController<ITarifficationTypesService, TarifficationType>
    {
        public TarifficationTypeController(ITarifficationTypesService service) : base(service)
        {
        }
        
    }
}