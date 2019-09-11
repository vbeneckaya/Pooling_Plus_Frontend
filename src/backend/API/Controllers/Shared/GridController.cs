using Domain.Services;

namespace API.Controllers.Shared
{
    public abstract class GridController<TService, TEntity, TDto> : DictonaryController<TService, TEntity, TDto> where TService : IDictonaryService<TEntity, TDto>
    {
        protected GridController(TService service) : base(service)
        {
        }
    }
}