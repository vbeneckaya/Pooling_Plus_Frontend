using DAL;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.Documents;
using Domain.Services.History;
using Domain.Services.UserIdProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Shared
{
    // TODO настроить foreignkeys
    public abstract class GridWithDocumentsBase<TEntity, TDto, TFormDto, TSummaryDto> : GridServiceBase<TEntity, TDto, TFormDto, TSummaryDto>, 
                                                                                        IGridWithDocuments<TEntity, TDto, TFormDto, TSummaryDto> 
        where TEntity : class, IPersistable, IWithDocumentsPersistable, new() 
        where TDto : IDto, new() 
        where TFormDto : IDto, new()
    {
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

            Guid? typeId = null;
            if (!string.IsNullOrEmpty(dto.TypeId) && Guid.TryParse(dto.TypeId, out Guid tId))
            {
                typeId = tId;
            }

            var document = new Document
            {
                Name = dto.Name,
                PersistableId = entity.Id,
                FileId = fileId,
                TypeId = typeId
            };

            _historyService.Save(id, "documentAttached", file.Name);

            db.Documents.Add(document);
            db.SaveChanges();

            return new ValidateResult
            {
                Id = document.Id.ToString()
            };
        }

        public ValidateResult UpdateDocument(Guid id, Guid documentId, DocumentDto dto)
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

            Guid? typeId = null;
            if (!string.IsNullOrEmpty(dto.TypeId) && Guid.TryParse(dto.TypeId, out Guid tId))
            {
                typeId = tId;
            }

            Document document = db.Documents.FirstOrDefault(x => x.Id == documentId);
            if (document == null)
            {
                return new ValidateResult("notFound");
            }

            document.Name = dto.Name;
            document.FileId = fileId;
            document.TypeId = typeId;

            db.Documents.Update(document);
            db.SaveChanges();

            return new ValidateResult();
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

            FileStorage file = db.FileStorage.FirstOrDefault(x => x.Id == document.FileId);
            db.FileStorage.Remove(file);

            _historyService.Save(id, "documentRemoved", file.Name);

            db.Documents.Remove(document);
            db.SaveChanges();

            return new ValidateResult();
        }

        protected GridWithDocumentsBase(AppDbContext appDbContext, IUserIdProvider userIdProvider, IHistoryService historyService) 
            : base(appDbContext, userIdProvider)
        {
            _historyService = historyService;
        }

        protected readonly IHistoryService _historyService;
    }
}
