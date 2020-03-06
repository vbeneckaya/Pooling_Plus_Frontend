using Domain.Persistables;
using Domain.Shared;
using System.Collections.Generic;
using System.IO;
using Domain.Services.UserProvider;

namespace Domain.Services.Shippings
{
    public interface IShippingsService : IGridService<Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto, ShippingFilterDto>
    {
        IEnumerable<LookUpDto> FindByNumber(NumberSearchFormDto dto);

        void InitializeNewShipping(Shipping shipping, CurrentUserDto currentUser);
       // IEnumerable<ShippingFormDto> FindByPoolingReservationId(NumberSearchFormDto dto);
        Stream ExportFormsToExcel(ExportExcelFormDto<ShippingFilterDto> dto);
        ImportResultDto ImportFormsFromExcel(Stream fileStream);
    }
}