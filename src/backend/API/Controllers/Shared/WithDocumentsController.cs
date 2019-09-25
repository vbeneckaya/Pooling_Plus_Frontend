using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.WithDocuments;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Shared
{
    public abstract class WithDocumentsController<TService, TEntity, TDto> : BaseServiceController<TService, TEntity, TDto> where TService : IWithDocumentsService where TEntity : IPersistable, IWithDocumentsPersistable where TDto : IDto
    {
        public WithDocumentsController(TService service) : base(service)
        {
        }

        [Route("{id}/documents")]
        [HttpGet]
        public IActionResult Get()
        {
            throw new NotImplementedException();
        }

        [Route("{id}/documents")]
        [HttpPost]
        public IActionResult Create()
        {
            throw new NotImplementedException();
        }

        [Route("{id}/documents")]
        [HttpDelete]
        public IActionResult Delete()
        {
            throw new NotImplementedException();
        }
    }
}