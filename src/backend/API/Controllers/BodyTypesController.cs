using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.BodyTypes;
using Domain.Services.DocumentTypes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/bodyTypes")]
    public class BodyTypesController : DictionaryController<IBodyTypesService, BodyType, BodyTypeDto>
    {
        public BodyTypesController(IBodyTypesService service) : base(service) { }

    }
}
