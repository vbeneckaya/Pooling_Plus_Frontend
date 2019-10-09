using Domain.Shared.FormFilters;

namespace Domain.Shared
{
    public class FilterForm<TFilter> : PagingForm
        where TFilter: SearchFilterDto
    {
        public TFilter Filter { get; set; }
    }
}