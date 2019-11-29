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

        ImportResultDto Import(IEnumerable<TDto> entityFrom);

        ImportResultDto ImportFromExcel(Stream fileStream);

        Stream ExportToExcel(SearchFormDto form);

        ValidateResult SaveOrCreate(TDto entityFrom);
        TDto Get(Guid id);

        ValidateResult Delete(Guid id);
    }
}