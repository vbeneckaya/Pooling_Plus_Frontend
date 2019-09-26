using Domain.Persistables;
using Domain.Services;
using Domain.Services.Documents;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace API.Controllers.Shared
{
    public abstract class GridWithDocumentsController<TService, TEntity, TDto, TFormDto> : GridController<TService, TEntity, TDto, TFormDto> 
        where TService : IGridWithDocuments<TEntity, TDto, TFormDto> where TEntity : IWithDocumentsPersistable, IPersistable
    {
        public GridWithDocumentsController(TService service) : base(service) { }

        [Route("{id}/documents")]
        [HttpGet]
        public IActionResult GetDocuments(Guid id)
        {
            try
            {
                IEnumerable<DocumentDto> documents = service.GetDocuments(id);

                return Ok(documents);
            }
            catch(Exception e)
            {
                return StatusCode(500);
            }
        }

        [Route("{id}/documents")]
        [HttpPost]
        public IActionResult CreateDocument(Guid id, [FromBody] DocumentDto dto)
        {
            try
            {
                ValidateResult result = service.CreateDocument(id, dto);

                return Ok(result);
            }
            catch(Exception e)
            {
                return StatusCode(500);
            }
        }

        [Route("{id}/documents/{documentId}")]
        [HttpDelete]
        public IActionResult DeleteDocument(Guid id, Guid documentId)
        {
            try
            {
                ValidateResult result = service.DeleteDocument(id, documentId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
    }
}