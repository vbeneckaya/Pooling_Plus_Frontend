using System;

namespace Domain.Persistables
{
    public class Role : IPersistableWithName
    {
        public Guid Id { get; set; }
        //public List<PermissionType> Permissions { get; set; }
        public string Name { get; set; }
    }
}