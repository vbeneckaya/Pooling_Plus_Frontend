using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.DocumentTypes;
using Domain.Services.UserProvider;
using Domain.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.DocumentTypes
{

    public class DocumentTypesService : DictonaryServiceBase<DocumentType, DocumentTypeDto>, IDocumentTypesService
    {
        public DocumentTypesService(ICommonDataService dataService, IUserProvider userProvider) : base(dataService, userProvider) { }

        public override ValidateResult MapFromDtoToEntity(DocumentType entity, DocumentTypeDto dto)
        {
            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive;

            return new ValidateResult(null, entity.Id.ToString());
        }

        public override IEnumerable<LookUpDto> ForSelect()
        {
            var entities = _dataService.GetDbSet<DocumentType>()
                .Where(i => i.IsActive)
                .OrderBy(x => x.Name)
                .ToList();

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
                Name = entity.Name,
                IsActive = entity.IsActive
            };
        }

        protected override IQueryable<DocumentType> ApplySort(IQueryable<DocumentType> query, SearchFormDto form)
        {
            return query
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id);
        }
    }
}
