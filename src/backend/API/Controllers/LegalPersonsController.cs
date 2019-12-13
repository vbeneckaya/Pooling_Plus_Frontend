using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.BodyTypes;
using Domain.Services.DocumentTypes;
using Domain.Services.LegalPersons;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/legalPerson")]
    public class LegalPersonsController : DictionaryController<ILegalPersonsService, LegalPerson, LegalPersonDto>
    {
        public LegalPersonsController(ILegalPersonsService service) : base(service) { }

    }
}
