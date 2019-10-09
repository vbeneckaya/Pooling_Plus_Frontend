using Domain.Persistables;
using Domain.Services.Documents;
using Domain.Shared;
using System;
using System.Collections.Generic;

namespace Domain.Services
{
    public interface IGridWithDocuments<TEntity, TDto, TFormDto, TSummaryDto> : IGridService<TEntity, TDto, TFormDto, TSummaryDto> 
        where TEntity : IWithDocumentsPersistable
    {
        IEnumerable<DocumentDto> GetDocuments(Guid id);
        ValidateResult CreateDocument(Guid id, DocumentDto dto);
        ValidateResult UpdateDocument(Guid id, Guid documentId, DocumentDto dto);
        ValidateResult DeleteDocument(Guid id, Guid documentId);
    }
}
