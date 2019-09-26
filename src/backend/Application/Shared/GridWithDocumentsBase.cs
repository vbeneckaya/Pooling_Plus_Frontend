using DAL;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.Documents;
using Domain.Services.UserIdProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;

namespace Application.Shared
{
    public abstract class GridWithDocumentsBase<TEntity, TDto> : GridServiceBase<TEntity, TDto>, IGridWithDocuments<TEntity, TDto> where TEntity : class, IPersistable, IWithDocumentsPersistable, new() where TDto : IDto
    {
        protected GridWithDocumentsBase(AppDbContext appDbContext, IUserIdProvider userIdProvider) : base(appDbContext, userIdProvider) { }

        public ValidateResult CreateDocument(Guid id, DocumentDto dto)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DocumentDto> GetDocuments(Guid id)
        {
            throw new NotImplementedException();
        }

        public ValidateResult DeleteDocument(Guid id, Guid documentId)
        {
            throw new NotImplementedException();
        }
    }
}
