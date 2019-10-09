using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Shippings;
using Domain.Shared;
using Domain.Shared.FormFilters;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/shippings")]
    public class ShippingsController : GridWithDocumentsController<IShippingsService, ShippingDto, ShippingFormDto, FilterForm<SearchFilter>> 
    {
        public ShippingsController(IShippingsService shippingsService) : base(shippingsService)
        {
        }
    }
}