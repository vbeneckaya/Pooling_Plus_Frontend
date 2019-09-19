using System.Collections.Generic;
using Domain.Enums;
using Domain.Persistables;

namespace Domain
{
    public interface IAppAction<T> : IAction
    {
        AppColor Color { get; set; }

        bool Run(User user, T entity);
        bool IsAvalible(Role role, T entity);
    }
    public interface IGroupAppAction<T> : IAction
    {
        AppColor Color { get; set; }

        bool Run(User user, IEnumerable<T> entity);
        bool IsAvalible(Role role, IEnumerable<T> entity);
    }

    public interface IAction
    {
    }
}