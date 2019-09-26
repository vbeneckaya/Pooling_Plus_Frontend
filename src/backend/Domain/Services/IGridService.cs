using System;
using System.Collections.Generic;
using System.IO;
using Domain.Shared;

namespace Domain.Services
{
    public interface IGridService<TEntity, TDto> : IService
    {
        SearchResult<TDto> Search(SearchForm form);
        IEnumerable<LookUpDto> ForSelect();
        ValidateResult SaveOrCreate(TDto entityFrom);
        IEnumerable<ValidateResult> Import(IEnumerable<TDto> entityFrom);
        ValidateResult ImportFromExcel(Stream fileStream);
        TDto Get(Guid id);
        
        IEnumerable<ActionDto> GetActions(IEnumerable<Guid> ids);
        AppActionResult InvokeAction(string actionName, IEnumerable<Guid> ids);
    }
}