using System;
using Domain.Persistables;
using Domain.Services.Identity;

namespace Domain.Services.UserIdProvider
{
    public interface IUserIdProvider
    {
        Guid? GetCurrentUserId();
        User GetCurrentUser();
    }
}