using Domain.Persistables;
using Domain.Shared;
using Domain.Shared.FormFilters;

namespace Domain.Services.Shippings
{
    public interface IShippingsService : IGridWithDocuments<Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto>
    {
    }
}