﻿using Domain.Persistables;
using Domain.Services;
using Domain.Services.Documents;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;

namespace API.Controllers.Shared
{
    /// <summary>
    /// Грид поддерживающий работу с документами
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TFormDto"></typeparam>
    public abstract class GridWithDocumentsController<TService, TEntity, TDto, TFormDto, TSummaryDto> : GridController<TService, TEntity, TDto, TFormDto, TSummaryDto>
        where TService : IGridWithDocuments<TEntity, TDto, TFormDto, TSummaryDto> 
        where TEntity : IWithDocumentsPersistable, IPersistable
    {
        public GridWithDocumentsController(TService service) : base(service) { }

        /// <summary>
        /// Получить документы
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/documents")]
        [HttpGet]
        public IActionResult GetDocuments(Guid id)
        {
            try
            {
                IEnumerable<DocumentDto> documents = service.GetDocuments(id);

                return Ok(documents);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to Get document");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Создать документ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Route("{id}/documents")]
        [HttpPost]
        public IActionResult CreateDocument(Guid id, [FromBody] DocumentDto dto)
        {
            try
            {
                ValidateResult result = service.CreateDocument(id, dto);

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to Create document");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Обновление документа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="documentId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Route("{id}/documents/{documentId}")]
        [HttpPut]
        public IActionResult UpdateDocument(Guid id, Guid documentId, [FromBody] DocumentDto dto)
        {
            try
            {
                ValidateResult result = service.UpdateDocument(id, documentId, dto);

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to Update document {documentId}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Удалить документ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [Route("{id}/documents/{documentId}")]
        [HttpDelete]
        public IActionResult DeleteDocument(Guid id, Guid documentId)
        {
            try
            {
                ValidateResult result = service.DeleteDocument(id, documentId);

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to Delete document {documentId}");
                return StatusCode(500);
            }
        }
    }
}