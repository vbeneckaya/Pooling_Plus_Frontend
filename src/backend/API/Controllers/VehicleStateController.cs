using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/vehicleState")]
    public class VehicleStateController : StateController<VehicleState>
    {
        
    }
}