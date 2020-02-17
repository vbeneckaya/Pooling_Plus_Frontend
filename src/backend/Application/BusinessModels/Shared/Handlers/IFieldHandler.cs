namespace Application.BusinessModels.Shared.Handlers
{
    public interface IFieldHandler<TEntity, TValue>
    {
        string ValidateChange(TEntity entity, TValue oldValue, TValue newValue);
        void AfterChange(TEntity entity, TValue oldValue, TValue newValue);
    }
}
