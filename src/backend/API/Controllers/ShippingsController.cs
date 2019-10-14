using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Documents;
using Domain.Services.Shippings;
using Domain.Shared;
using Domain.Shared.FormFilters;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/shippings")]
    public class ShippingsController : GridWithDocumentsController<IShippingsService, Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto, FilterFormDto<SearchFilterDto>> 
    {
        public ShippingsController(IShippingsService shippingsService, IDocumentService documentService) : base(shippingsService, documentService)
        {
        }
    }
}