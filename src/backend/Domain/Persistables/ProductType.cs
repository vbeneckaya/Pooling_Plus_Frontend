using System;

namespace Domain.Persistables
{
    public class ProductType : IPersistableWithName
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string PoolingId { get; set; }

        public bool IsActive { get; set; }
    }
}
