using System;
using System.Collections.Generic;
using Domain.Shared;

namespace Domain.Services
{
    public interface IGridService<TEntity, TDto> : IService
    {
        SearchResult<TDto> Search(SearchForm form);
        ValidateResult SaveOrCreate(TDto entityFrom);
        IEnumerable<ValidateResult> Import(IEnumerable<TDto> entityFrom);
        TDto Get(Guid id);
        
        IEnumerable<ActionDto> GetActions(IEnumerable<Guid> ids);
        AppActionResult InvokeAction(string actionName, IEnumerable<Guid> ids);
    }
}