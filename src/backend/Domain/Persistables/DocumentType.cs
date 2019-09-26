using System;

namespace Domain.Persistables
{
    public class DocumentType : IPersistableWithName
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
