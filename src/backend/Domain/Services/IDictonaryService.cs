using System;
using System.Collections.Generic;
using Infrastructure.Shared;

namespace Domain.Services
{
    public interface IDictonaryService<TEntity, TDto> : IService
    {
        IEnumerable<TDto> Search(SearchForm form);
        ValidateResult SaveOrCreate(TDto entityFrom);
        TDto Get(Guid id);
    }
}