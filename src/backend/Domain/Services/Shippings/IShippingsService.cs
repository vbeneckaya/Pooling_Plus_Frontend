using Domain.Persistables;

namespace Domain.Services.Shippings
{
    public interface IShippingsService : IGridWithDocuments<ShippingDto, ShippingFormDto>
    {
    }
}