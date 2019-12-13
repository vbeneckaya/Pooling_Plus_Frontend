using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.BodyTypes;
using Domain.Services.DocumentTypes;
using Domain.Services.Companies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/company")]
    public class CompaniesController : DictionaryController<ICompaniesService, Company, CompanyDto>
    {
        public CompaniesController(ICompaniesService service) : base(service) { }

    }
}
