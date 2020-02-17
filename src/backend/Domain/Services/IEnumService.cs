using System.Collections.Generic;
using Domain.Shared;

namespace Domain.Services
{
    public interface IEnumService<TEnum> : IService 
    {
        IEnumerable<LookUpDto> ForSelect();
    }
}