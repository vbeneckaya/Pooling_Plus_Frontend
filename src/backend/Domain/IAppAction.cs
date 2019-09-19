using System.Collections.Generic;
using Domain.Enums;
using Domain.Persistables;

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

        bool Run(User user, T target);
        bool IsAvalible(Role role, T target);
    }
}