using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.TransportСompanies;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("TransportСompanies")]
    public class TransportСompaniesController : DictonaryController<ITransportСompaniesService, TransportСompany, TransportСompanyDto> 
    {
        public TransportСompaniesController(ITransportСompaniesService transportСompaniesService) : base(transportСompaniesService)
        {
        }
    }
}