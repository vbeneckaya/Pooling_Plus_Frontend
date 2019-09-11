using System;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using Domain.Services.UserIdProvider;
using Microsoft.AspNetCore.Http;

namespace Application.Services.UserIdProvider
{

    public class UserIdProvider : IUserIdProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppDbContext db;

        public UserIdProvider(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            this.httpContextAccessor = httpContextAccessor;
            db = dbContext;
        }

        public Guid? GetCurrentUserId()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var userIdClaim = httpContext.User.FindFirst("userId");
            return userIdClaim != null
                ? Guid.Parse(userIdClaim.Value)
                : (Guid?)null;
        }

        public User GetCurrentUser()
        {
            return db.Users.GetById(EnsureCurrentUserId());
        }
        
        public Guid EnsureCurrentUserId()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                throw new NullReferenceException("Невозможно определить текущего пользователя");

            return userId.Value;
        }
    }
}