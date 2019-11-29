using Domain.Services.Tonnages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Tonnages
{
    public interface ITonnagesService : IDictonaryService<Domain.Persistables.Tonnage, TonnageDto>
    {
    }
}
