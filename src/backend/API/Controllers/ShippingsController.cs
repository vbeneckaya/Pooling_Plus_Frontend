using API.Controllers.Shared;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.Documents;
using Domain.Services.Shippings;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.IO;
using System.Linq;
using Domain.Extensions;

namespace API.Controllers
{
    [Route("api/shippings")]
    [GridPermissions(Search = RolePermissions.ShippingsView, SaveOrCreate = RolePermissions.ShippingsView)]
    public class ShippingsController : GridWithDocumentsController<IShippingsService, Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto, ShippingFilterDto> 
    {
        public ShippingsController(IShippingsService shippingsService, IDocumentService documentService) : base(shippingsService, documentService)
        {
        }

        /// <summary>
        /// Поиск по номеру
        /// </summary>
        [HttpPost("findNumber")]
        public IActionResult Search([FromBody]NumberSearchFormDto form)
        {
            try
            {
                var result = service.FindByNumber(form);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to Find Shipping number");
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Импортировать список форм из excel
        /// </summary>
        [HttpPost("importFormsFromExcel")]
        public ImportResultDto ImportFormsFromExcel()
        {
            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            using (var stream = new FileStream(Path.GetTempFileName(), FileMode.Create))
            {
                file.CopyTo(stream);
                return service.ImportFormsFromExcel(stream);
            }
        }
        
        
        /// <summary>
        /// Экспортировать список форм в excel
        /// </summary>
        [HttpPost("exportFormsToExcel"), DisableRequestSizeLimit]
        public IActionResult ExportFormsToExcel([FromBody]ExportExcelFormDto<ShippingFilterDto> dto) {
            
            var memoryStream = service.ExportFormsToExcel(dto);
            return File(memoryStream, "application/vnd.ms-excel", $"Export {typeof(Shipping).Name.Pluralize()} {DateTime.Now.ToString("dd.MM.yy HH.mm")}.xlsx");
        }
        
        /// <summary>
        /// Импортировать список форм из Pooling для поставщика
        /// </summary>
        [HttpGet("importFormsFromPooling")]
        public IActionResult ImportFormsFromPooling([FromQuery] string providerId) {
            service.ImportFormsFromPooling(providerId);
            return Ok();
        }
    }
}