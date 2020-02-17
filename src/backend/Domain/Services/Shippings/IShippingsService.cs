using Domain.Persistables;
using Domain.Shared;
using System.Collections.Generic;

namespace Domain.Services.Shippings
{
    public interface IShippingsService : IGridService<Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto, ShippingFilterDto>
    {
        IEnumerable<LookUpDto> FindByNumber(NumberSearchFormDto dto);
    }
}