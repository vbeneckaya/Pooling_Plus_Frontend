using System;
using System.Collections.Generic;
using Domain.Shared;

namespace Domain.Services
{
    public interface IGridService<TEntity, TDto> : IService
    {
        IEnumerable<TDto> Search(SearchForm form);
        ValidateResult SaveOrCreate(TDto entityFrom);
        TDto Get(Guid id);
        
        IEnumerable<ActionDto> GetActions(IEnumerable<Guid> ids);
    }
}