using System.Collections.Generic;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.UserProvider;

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

        AppActionResult Run(CurrentUserDto user, T target);
        bool IsAvailable(Role role, T target);
    }
}