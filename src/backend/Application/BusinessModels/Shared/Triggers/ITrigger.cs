namespace Application.BusinessModels.Shared.Triggers
{
    public interface ITrigger<TEntity>
    {
        bool IsTriggered(TEntity entity);
        void Execute(TEntity entity);
    }
}
