﻿using Domain.Persistables;
using Domain.Services;
using Domain.Services.Documents;
using Domain.Shared;
using Domain.Persistables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Services.History;
using DAL.Services;

namespace Application.Services.Documents
{
    public class DocumentService: IDocumentService
    {
        private readonly ICommonDataService dataService;

        private readonly IHistoryService _historyService;


        public DocumentService(ICommonDataService dataService, IHistoryService historyService)
        {
            this.dataService = dataService;
            _historyService = historyService;
        }

        public ValidateResult CreateDocument(Guid id, DocumentDto dto)
        {
            //TEntity entity = this.dataService.GetById<TEntity>(id);
            //if (entity == null)
            //{
            //    return new ValidateResult("notFound");
            //}

            bool tryParseFileId = Guid.TryParse(dto.FileId, out Guid fileId);
            FileStorage file = dataService.GetDbSet<FileStorage>().FirstOrDefault(x => x.Id == fileId);
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
                PersistableId = id,
                FileId = fileId,
                TypeId = typeId
            };

            _historyService.Save(id, "documentAttached", file.Name);

            dataService.GetDbSet<Document>().Add(document);
            dataService.SaveChanges();

            return new ValidateResult
            {
                Id = document.Id.ToString()
            };
        }

        public ValidateResult UpdateDocument(Guid id, Guid documentId, DocumentDto dto)
        {
            //TEntity entity = this.dataService.GetById<TEntity>(id);
            //if (entity == null)
            //{
            //    return new ValidateResult("notFound");
            //}

            bool tryParseFileId = Guid.TryParse(dto.FileId, out Guid fileId);
            FileStorage file = dataService.GetDbSet<FileStorage>().FirstOrDefault(x => x.Id == fileId);
            if (!tryParseFileId || file == null)
            {
                return new ValidateResult("notFound");
            }

            Guid? typeId = null;
            if (!string.IsNullOrEmpty(dto.TypeId) && Guid.TryParse(dto.TypeId, out Guid tId))
            {
                typeId = tId;
            }

            Document document = dataService.GetDbSet<Document>().FirstOrDefault(x => x.Id == documentId);
            if (document == null)
            {
                return new ValidateResult("notFound");
            }

            document.Name = dto.Name;
            document.FileId = fileId;
            document.TypeId = typeId;

            dataService.GetDbSet<Document>().Update(document);
            dataService.SaveChanges();

            return new ValidateResult();
        }

        public IEnumerable<DocumentDto> GetDocuments(Guid id)
        {
            return dataService.GetDbSet<Document>().Where(x => x.PersistableId == id)
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
            Document document = dataService.GetDbSet<Document>().FirstOrDefault(x => x.Id == documentId);
            if (document == null)
            {
                return new ValidateResult("notFound");
            }

            FileStorage file = dataService.GetDbSet<FileStorage>().FirstOrDefault(x => x.Id == document.FileId);
            dataService.GetDbSet<FileStorage>().Remove(file);

            _historyService.Save(id, "documentRemoved", file.Name);

            dataService.GetDbSet<Document>().Remove(document);
            dataService.SaveChanges();

            return new ValidateResult();
        }
    }
}
