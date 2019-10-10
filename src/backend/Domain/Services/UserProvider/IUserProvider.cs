using System;

namespace Domain.Services.UserProvider
{
    public interface IUserProvider
    {
        Guid? GetCurrentUserId();
        CurrentUserDto GetCurrentUser();
    }
}