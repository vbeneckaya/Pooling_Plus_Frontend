using System;

namespace Domain.Persistables
{
    public class Client : IPersistableWithName
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string PoolingId { get; set; }

        public Guid? CompanyId { get; set; }

        public bool IsActive { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
