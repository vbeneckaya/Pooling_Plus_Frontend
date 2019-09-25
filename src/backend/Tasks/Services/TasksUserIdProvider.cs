using Domain.Persistables;
using Domain.Services.UserIdProvider;
using System;

namespace Tasks.Services
{
    public class TasksUserIdProvider : IUserIdProvider
    {
        public User GetCurrentUser()
        {
            return null;
        }

        public Guid? GetCurrentUserId()
        {
            return null;
        }
    }
}
