using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Clients;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/clients")]
    public class ClientsController : DictionaryController<IClientsService, Client, ClientDto>
    {
        public ClientsController(IClientsService clientsService) : base(clientsService)
        {
        }
    }
}
