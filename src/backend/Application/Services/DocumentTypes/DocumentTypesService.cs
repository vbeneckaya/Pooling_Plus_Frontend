using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.DocumentTypes;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.DocumentTypes
{
    public class DocumentTypesService : DictonaryServiceBase<DocumentType, DocumentTypeDto>, IDocumentTypesService
    {
        public DocumentTypesService(AppDbContext appDbContext) : base(appDbContext) { }

        public override void MapFromDtoToEntity(DocumentType entity, DocumentTypeDto dto)
        {
            entity.Name = dto.Name;
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = db.DocumentTypes.OrderBy(x => x.Name).ToList();
            foreach (var entity in entities)
            {
                yield return new LookUpDto
                {
                    Name = entity.Name,
                    Value = entity.Id.ToString()
                };
            }
        }

        public override DocumentTypeDto MapFromEntityToDto(DocumentType entity)
        {
            return new DocumentTypeDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name
            };
        }

        public override DbSet<DocumentType> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.DocumentTypes;
        }
    }
}
