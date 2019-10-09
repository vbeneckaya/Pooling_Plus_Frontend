using Domain.Services.History;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;

namespace API.Controllers
{
    /// <summary>
    /// История
    /// </summary>
    [Route("api/history")]
    public class HistoryController : Controller
    {
        /// <summary>
        /// Получить историю сущности
        /// </summary>
        /// <param name="entityId">Идентификатор сущности</param>
        /// <param name="lang">Язык для локализации</param>
        /// <returns></returns>
        [Route("{entityId}/{lang}")]
        [HttpGet]
        public IActionResult Get(Guid entityId, string lang)
        {
            try
            {
                HistoryDto dto = _historyService.Get(entityId, lang);

                return Json(dto);
            }
            catch(Exception e)
            {
                Log.Error(e, $"Failed to Get history enties {entityId}");
                return StatusCode(500);
            }
        }

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        private readonly IHistoryService _historyService;
    }
}