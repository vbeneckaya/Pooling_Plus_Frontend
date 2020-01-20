using System;

namespace Domain.Persistables
{
    public class Role : IPersistableWithName
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public int[] Permissions { get; set; }
        public string[] Actions { get; set; }

        public Guid? CompanyId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}