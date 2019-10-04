using System;

namespace Domain.Persistables
{
    public class VehicleType : IPersistableWithName
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
