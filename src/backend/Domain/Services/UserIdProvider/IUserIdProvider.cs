using System;
using Domain.Persistables;

namespace Domain.Services.UserIdProvider
{
    public interface IUserIdProvider
    {
        Guid? GetCurrentUserId();
        User GetCurrentUser();
    }
}