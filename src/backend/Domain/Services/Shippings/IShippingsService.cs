using Domain.Persistables;
using Domain.Shared;
using System.Collections.Generic;
using System.IO;

namespace Domain.Services.Shippings
{
    public interface IShippingsService : IGridService<Shipping, ShippingDto, ShippingFormDto, ShippingSummaryDto, ShippingFilterDto>
    {
        IEnumerable<LookUpDto> FindByNumber(NumberSearchFormDto dto);
        IEnumerable<ShippingFormDto> FindByPoolingReservationId(NumberSearchFormDto dto);
        Stream ExportFormsToExcel(ExportExcelFormDto<ShippingFilterDto> dto);
        ImportResultDto ImportFormsFromExcel(Stream fileStream);
    }
}