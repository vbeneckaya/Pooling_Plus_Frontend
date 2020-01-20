using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.ProductTypes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Route("api/productTypes")]
    public class ProductTypesController : DictionaryController<IProductTypesService, ProductType, ProductTypeDto>
    {
        public ProductTypesController(IProductTypesService service) : base(service) { }
    }
}
