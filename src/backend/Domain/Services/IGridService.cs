using System;
using System.Collections.Generic;
using System.IO;
using Domain.Shared;

namespace Domain.Services
{
    public interface IGridService<TEntity, TDto, TFormDto, TSummaryDto, TSearchForm> : IService
    {
        SearchResult<TDto> Search(TSearchForm form);
        IEnumerable<string> SearchIds(TSearchForm form);
        IEnumerable<LookUpDto> ForSelect();
        ValidateResult SaveOrCreate(TFormDto entityFrom);
        TDto Get(Guid id);
        TFormDto GetForm(Guid id);

        TSummaryDto GetSummary(IEnumerable<Guid> ids);

        AppActionResult InvokeAction(string actionName, IEnumerable<Guid> ids);
        IEnumerable<ActionDto> GetActions(IEnumerable<Guid> ids);

        AppActionResult InvokeBulkUpdate(string fieldName, IEnumerable<Guid> ids, string value);
        IEnumerable<BulkUpdateDto> GetBulkUpdates(IEnumerable<Guid> ids);

        IEnumerable<ValidateResult> Import(IEnumerable<TFormDto> entityFrom);
        ValidateResult ImportFromExcel(Stream fileStream);
        Stream ExportToExcel(ExportExcelFormDto dto);
    }
}