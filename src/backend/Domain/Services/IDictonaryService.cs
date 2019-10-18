using System;
using System.Collections.Generic;
using System.IO;
using Domain.Shared;

namespace Domain.Services
{
    public interface IDictonaryService<TEntity, TDto> : IService
    {
        SearchResult<TDto> Search(SearchFormDto form);
        IEnumerable<LookUpDto> ForSelect();
        IEnumerable<ValidateResult> Import(IEnumerable<TDto> entityFrom);
        ValidateResult ImportFromExcel(Stream fileStream);
        Stream ExportToExcel();
        ValidateResult SaveOrCreate(TDto entityFrom);
        TDto Get(Guid id);
    }
}