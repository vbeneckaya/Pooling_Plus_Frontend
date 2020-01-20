using Domain.Persistables;
using System.Collections.Generic;

namespace Domain.Shared
{
    public class EntityChanges
    {
        public IPersistable Entity { get; set; }

        public List<EntityFieldChanges> FieldChanges { get; set; }
    }
}
