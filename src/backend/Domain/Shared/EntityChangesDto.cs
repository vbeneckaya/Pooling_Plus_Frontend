using Domain.Persistables;
using System.Collections.Generic;

namespace Domain.Shared
{
    public class EntityChangesDto<TEntity> where TEntity : class, IPersistable
    {
        public TEntity Entity { get; set; }
        public List<EntityFieldChangesDto> FieldChanges { get; set; }
    }
}
