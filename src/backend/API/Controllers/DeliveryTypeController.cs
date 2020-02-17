using Domain.Enums;
using Domain.Services.DeliveryTypes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/deliveryType")]
    public class DeliveryTypeController : EnumController<IDeliveryTypesService,DeliveryType>
    {
        public DeliveryTypeController(IDeliveryTypesService service) : base(service)
        {
        }
    }
}