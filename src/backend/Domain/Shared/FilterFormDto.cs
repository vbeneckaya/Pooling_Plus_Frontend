using Domain.Shared.FormFilters;

namespace Domain.Shared
{
    public class FilterFormDto<TFilter> : PagingFormDto
        where TFilter: SearchFilterDto
    {
        public TFilter Filter { get; set; }
    }
}