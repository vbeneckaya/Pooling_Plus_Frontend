using Domain.Enums;
using Domain.Persistables;

namespace Domain
{
    public interface IAppAction<T>
    {
        AppColor Color { get; set; }

        bool Run(User user, T entity);
        bool IsAvalible(Role role, T entity);
    }
}