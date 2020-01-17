using Domain.Enums;
using Domain.Services.Report;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/report")]
    //[HasPermission(RolePermissions.Report)]
    public class ReportController : Controller
    {
        private readonly IReportService reportService;
        public ReportController(IReportService reportService)
        {
            this.reportService = reportService;
        }
        
        /// <summary>
        /// Получение конфига отчёта 
        /// </summary>
        [HttpGet("config")]
        public EmbeddedReportConfig GetConfig()
        {
            return reportService.GetConfig();
        }
    }
}