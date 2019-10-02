using System.Collections.Generic;
using Domain.Shared;

namespace Domain.Services.PickingTypes
{
    public interface IPickingTypesService
    {
        IEnumerable<LookUpDto> ForSelect();
    }
}