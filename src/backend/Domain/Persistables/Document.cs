using System;

namespace Domain.Persistables
{
    public class Document : IPersistable
    {
        public Guid Id { get; set; }
    }
}
