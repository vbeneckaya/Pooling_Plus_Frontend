﻿using DAL;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.Documents;
using Domain.Services.UserIdProvider;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Shared
{
    public abstract class GridWithDocumentsBase<TEntity, TDto> : GridServiceBase<TEntity, TDto>, IGridWithDocuments<TEntity, TDto> where TEntity : class, IPersistable, IWithDocumentsPersistable, new() where TDto : IDto
    {
        protected GridWithDocumentsBase(AppDbContext appDbContext, IUserIdProvider userIdProvider) : base(appDbContext, userIdProvider) { }

        public ValidateResult CreateDocument(Guid id, DocumentDto dto)
        {
            TEntity entity = UseDbSet(db)
                .Include(_ => _.Documents)
                .FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                return new ValidateResult("notFound");
            }

            // TODO проверить существование файла
            // TODO проверить существование типа документа

            // TODO убрать\оставить Parse в Guid
            var document = new Document
            {
                Name = dto.Name,
                FileId = Guid.Parse(dto.FileId),
                TypeId = Guid.Parse(dto.FileId)
            };
            entity.Documents.Add(document);

            db.Update(entity);
            db.SaveChanges();

            return new ValidateResult();
        }

        public IEnumerable<DocumentDto> GetDocuments(Guid id)
        {
            return UseDbSet(db)
                .Include(_ => _.Documents)
                .FirstOrDefault(x => x.Id == id)
                ?.Documents
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
            TEntity entity = UseDbSet(db)
                .Include(_ => _.Documents)
                .FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                return new ValidateResult("notFound");
            }

            Document document = entity.Documents.FirstOrDefault(x => x.Id == documentId);
            if (document == null)
            {
                return new ValidateResult("notFound");
            }

            entity.Documents.Remove(document);

            db.Update(entity);
            db.SaveChanges();

            return new ValidateResult();
        }
    }
}
