using Domain.Persistables;

namespace Domain.Services.Shippings
{
    public interface IShippingsService : IGridWithDocuments<Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto>
    {
    }
}