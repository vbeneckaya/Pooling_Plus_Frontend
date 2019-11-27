using Domain.Persistables;
using Domain.Shared;

namespace Application.BusinessModels.Shared.Triggers
{
    public interface ITrigger<TEntity> where TEntity : class, IPersistable
    {
        bool IsTriggered(EntityChangesDto<TEntity> changes);
        void Execute(TEntity entity);
    }
}
