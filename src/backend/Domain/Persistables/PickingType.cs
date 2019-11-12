using System;

namespace Domain.Persistables
{
    public class PickingType : IPersistableWithName
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
