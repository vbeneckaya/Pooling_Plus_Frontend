using System.Collections.Generic;

namespace Application.BusinessModels.Shared.Actions
{
    public interface IGroupAppAction<T> : IAction<IEnumerable<T>>
    {
    }
}