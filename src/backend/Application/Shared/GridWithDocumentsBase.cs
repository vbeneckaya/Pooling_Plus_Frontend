using DAL;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.Documents;
using Domain.Services.UserIdProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Shared
{
    // TODO настроить foreignkeys

    public abstract class GridWithDocumentsBase<TEntity, TDto, TFormDto> : GridServiceBase<TEntity, TDto, TFormDto>, IGridWithDocuments<TEntity, TDto, TFormDto>
        where TEntity : class, IPersistable, IWithDocumentsPersistable, new() where TDto : IDto where TFormDto : IDto
    {
        protected GridWithDocumentsBase(AppDbContext appDbContext, IUserIdProvider userIdProvider) : base(appDbContext, userIdProvider) { }

        public ValidateResult CreateDocument(Guid id, DocumentDto dto)
        {
            TEntity entity = UseDbSet(db).FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                return new ValidateResult("notFound");
            }

            bool tryParseFileId = Guid.TryParse(dto.FileId, out Guid fileId);
            FileStorage file = db.FileStorage.FirstOrDefault(x => x.Id == fileId);
            if (!tryParseFileId || file == null)
            {
                return new ValidateResult("notFound");
            }
            bool tryParseTypeId = Guid.TryParse(dto.TypeId, out Guid typeId);
            DocumentType type = db.DocumentTypes.FirstOrDefault(x => x.Id == typeId);
            if (!tryParseTypeId || type == null)
            {
                return new ValidateResult("notFound");
            }

            var document = new Document
            {
                Name = dto.Name,
                PersistableId = entity.Id,
                FileId = fileId,
                TypeId = typeId
            };

            db.Documents.Add(document);
            db.SaveChanges();

            return new ValidateResult
            {
                Id = document.Id.ToString()
            };
        }

        public IEnumerable<DocumentDto> GetDocuments(Guid id)
        {
            return db.Documents.Where(x => x.PersistableId == id)
                .Select(s => new DocumentDto
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    FileId = s.FileId.ToString(),
                    TypeId = s.TypeId.ToString()
                })
                .ToList();
        }

        public ValidateResult DeleteDocument(Guid id, Guid documentId)
        {
            Document document = db.Documents.FirstOrDefault(x => x.Id == documentId);
            if (document == null)
            {
                return new ValidateResult("notFound");
            }

            db.Documents.Remove(document);
            db.SaveChanges();

            return new ValidateResult();
        }
    }
}
