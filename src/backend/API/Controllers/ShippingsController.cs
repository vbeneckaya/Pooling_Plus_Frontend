using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Documents;
using Domain.Services.Shippings;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;

namespace API.Controllers
{
    [Route("api/shippings")]
    public class ShippingsController : GridWithDocumentsController<IShippingsService, Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto, ShippingFilterDto> 
    {
        public ShippingsController(IShippingsService shippingsService, IDocumentService documentService) : base(shippingsService, documentService)
        {
        }

        /// <summary>
        /// ����� �� ������
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
    }
}