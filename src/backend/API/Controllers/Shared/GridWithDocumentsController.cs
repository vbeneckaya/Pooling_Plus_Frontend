using Domain.Persistables;
using Domain.Services;
using Domain.Services.Documents;
using Microsoft.AspNetCore.Mvc;
using System;

namespace API.Controllers.Shared
{
    public abstract class GridWithDocumentsController<TService, TEntity, TDto> : GridController<TService, TEntity, TDto> where TService : IGridWithDocuments<TEntity, TDto> where TEntity : IWithDocumentsPersistable, IPersistable
    {
        public GridWithDocumentsController(TService service) : base(service) { }

        [Route("{id}/documents")]
        [HttpGet]
        public IActionResult GetDocuments(Guid id)
        {
            throw new NotImplementedException();
        }

        [Route("{id}/documents")]
        [HttpPost]
        public IActionResult CreateDocument(Guid id, DocumentDto dto)
        {
            throw new NotImplementedException();
        }

        [Route("{id}/documents/{documentId}")]
        [HttpDelete]
        public IActionResult DeleteDocument(Guid id, Guid documentId)
        {
            throw new NotImplementedException();
        }
    }
}