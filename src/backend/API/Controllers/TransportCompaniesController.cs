using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.TransportCompanies;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/transportCompanies")]
    public class TransportCompaniesController : DictonaryController<ITransportCompaniesService, TransportCompany, TransportCompanyDto> 
    {
        public TransportCompaniesController(ITransportCompaniesService transportCompaniesService) : base(transportCompaniesService)
        {
        }
    }
}