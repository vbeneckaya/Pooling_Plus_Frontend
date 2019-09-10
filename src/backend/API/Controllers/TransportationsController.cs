using Domain.Persistables;
using Domain.Services.Transportations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("Transportations")]
    public class TransportationsController : DictonaryController<ITransportationsService, Transportation, TransportationDto>
    {
        public TransportationsController(ITransportationsService transportationsService) : base(transportationsService)
        {
        }
    }
}