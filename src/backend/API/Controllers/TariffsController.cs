using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Tariffs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("Tariffs")]
    public class TariffsController : DictonaryController<ITariffsService, Tariff, TariffDto> 
    {
        public TariffsController(ITariffsService tariffsService) : base(tariffsService)
        {
        }
    }
}