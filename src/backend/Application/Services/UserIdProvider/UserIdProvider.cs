﻿using System;
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
            User user = db.Users.GetById(EnsureCurrentUserId());
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }
            return user;
        }
        
        public Guid EnsureCurrentUserId()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("Невозможно определить текущего пользователя");

            return userId.Value;
        }
    }
}