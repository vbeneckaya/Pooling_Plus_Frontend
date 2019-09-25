using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.DocumentTypes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/documentTypes")]
    public class DocumentTypesController : DictionaryController<IDocumentTypesService, DocumentType, DocumentTypeDto>
    {
        public DocumentTypesController(IDocumentTypesService service) : base(service) { }
    }
}