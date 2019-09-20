using System;
using System.Collections.Generic;
using Domain.Shared;

namespace Domain.Services
{
    public interface IDictonaryService<TEntity, TDto> : IService
    {
        SearchResult<TDto> Search(SearchForm form);
        IEnumerable<ValidateResult> Import(IEnumerable<TDto> entityFrom);
        ValidateResult SaveOrCreate(TDto entityFrom);
        TDto Get(Guid id);
    }
}