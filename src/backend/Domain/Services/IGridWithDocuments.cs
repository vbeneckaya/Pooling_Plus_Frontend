using Domain.Persistables;
using Domain.Services.Documents;
using Domain.Shared;
using System;
using System.Collections.Generic;

namespace Domain.Services
{
    public interface IGridWithDocuments<TEntity, TDto, TFormDto> : IGridService<TEntity, TDto, TFormDto> where TEntity : IWithDocumentsPersistable
    {
        IEnumerable<DocumentDto> GetDocuments(Guid id);
        ValidateResult CreateDocument(Guid id, DocumentDto dto);
        ValidateResult DeleteDocument(Guid id, Guid documentId);
    }
}
