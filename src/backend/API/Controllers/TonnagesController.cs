using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.BodyTypes;
using Domain.Services.DocumentTypes;
using Domain.Services.Tonnages;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/tonnages")]
    public class TonnagesController : DictionaryController<ITonnagesService, Tonnage, TonnageDto>
    {
        public TonnagesController(ITonnagesService service) : base(service) { }

    }
}
