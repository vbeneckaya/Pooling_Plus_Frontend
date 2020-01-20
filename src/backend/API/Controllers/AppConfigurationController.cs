using Domain.Services.AppConfiguration;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api")]
    public class AppConfigurationController : Controller
    {
        private readonly IAppConfigurationService appConfigurationService;

        public AppConfigurationController(IAppConfigurationService appConfigurationService)
        {
            this.appConfigurationService = appConfigurationService;
        }
        /// <summary>
        /// Получение конфигурации гридов и справочников
        /// </summary>
        [HttpGet("appConfiguration")] 
        public AppConfigurationDto Configuration()
        {
            return appConfigurationService.GetConfiguration();
        }
    }
}