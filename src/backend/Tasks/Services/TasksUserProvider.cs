using Domain.Persistables;
using Domain.Services.UserProvider;
using System;

namespace Tasks.Services
{
    public class TasksUserProvider : IUserProvider
    {
        public User GetCurrentUser()
        {
            return null;
        }

        public Guid? GetCurrentUserId()
        {
            return null;
        }

        public string GetCurrentUserLanguage()
        {
            return "ru";
        }
    }
}
