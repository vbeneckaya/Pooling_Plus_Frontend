using API.Controllers.Shared;
using Domain.Persistables;
using Microsoft.AspNetCore.Mvc;
using Domain.Services.Providers;

namespace API.Controllers
{
    [Route("api/providers")]
    public class ProvidersController : DictionaryController<IProvidersService, Provider, ProviderDto>
    {
        public ProvidersController(IProvidersService service) : base(service) { }

    }
}
