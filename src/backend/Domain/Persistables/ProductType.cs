using Domain.Extensions;
using System;

namespace Domain.Persistables
{
    public class ProductType : IPersistableWithName
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string PoolingId { get; set; }

        [ReferenceType(typeof(Company))]
        public Guid? CompanyId { get; set; }

        public bool IsActive { get; set; }
    }
}
