using System.Collections.Generic;

namespace Domain.Services.Warehouses
{
    public interface ISoldToService
    {
        IEnumerable<SoldToDto> ForSelect();
    }
}
