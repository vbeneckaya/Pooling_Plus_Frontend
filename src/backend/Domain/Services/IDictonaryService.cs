using System;
using System.Collections.Generic;
using System.IO;
using Domain.Shared;

namespace Domain.Services
{
    public interface IDictonaryService<TEntity, TDto> : IService
    {
        SearchResult<TDto> Search(SearchForm form);
        IEnumerable<LookUpDto> ForSelect();
        IEnumerable<ValidateResult> Import(IEnumerable<TDto> entityFrom);
        IEnumerable<ValidateResult> ImportFromExcel(Stream fileStream);
        ValidateResult SaveOrCreate(TDto entityFrom);
        TDto Get(Guid id);
    }
}