using System;
using Domain.Persistables;

namespace Domain.Services.UserProvider
{
    public interface IUserProvider
    {
        Guid? GetCurrentUserId();
        User GetCurrentUser();
        string GetCurrentUserLanguage();
    }
}