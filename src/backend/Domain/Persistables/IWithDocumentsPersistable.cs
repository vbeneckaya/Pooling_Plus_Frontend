using Domain.Persistables;
using System.Collections.Generic;

namespace Domain.Persistables
{
    public interface IWithDocumentsPersistable
    {
        IEnumerable<Document> Documents { get; set; }
    }
}
