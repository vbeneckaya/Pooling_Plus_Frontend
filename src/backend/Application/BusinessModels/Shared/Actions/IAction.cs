using Domain.Enums;
using Domain.Services;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Shared.Actions
{
    public interface IAction<T>
    {
        AppColor Color { get; set; }
        AppActionResult Run(CurrentUserDto user, T target);
        bool IsAvailable(T target);
    }
}