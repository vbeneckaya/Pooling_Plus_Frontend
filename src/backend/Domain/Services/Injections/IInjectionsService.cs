using Domain.Persistables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Injections
{
    public interface IInjectionsService : IDictonaryService<Injection, InjectionDto>
    {
        IEnumerable<InjectionDto> GetByTaskName(string taskName);
    }
}
