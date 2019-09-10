using System;
using System.Collections.Generic;
using Domain.Shared;

namespace Domain.Services.Users
{
    public interface IDictonaryService<TEntity, TDto> : IService
    {
        IEnumerable<TDto> Search(SearchForm form);
        ValidateResult SaveOrCreate(TDto entityFrom);
        TDto Get(Guid id);
    }
}