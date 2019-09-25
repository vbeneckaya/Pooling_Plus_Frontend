using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.DocumentTypes;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.DocumentTypes
{
    public class DocumentTypesService : DictonaryServiceBase<DocumentType, DocumentTypeDto>, IDocumentTypesService
    {
        public DocumentTypesService(AppDbContext appDbContext) : base(appDbContext) { }

        public override void MapFromDtoToEntity(DocumentType entity, DocumentTypeDto dto)
        {
            entity.Name = dto.Name;
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
