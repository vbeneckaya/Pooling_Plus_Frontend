using System.Collections.Generic;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Domain
{
    public interface IAppAction<T> : IAction<T>
    {
    }
    public interface IGroupAppAction<T> : IAction<IEnumerable<T>>
    {
    }

    public interface IAction<T>
    {
        AppColor Color { get; set; }

        AppActionResult Run(User user, T target);
        bool IsAvailable(Role role, T target);
    }
}