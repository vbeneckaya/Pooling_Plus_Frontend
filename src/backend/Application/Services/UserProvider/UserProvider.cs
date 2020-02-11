using System;
using AutoMapper;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using Domain.Services.UserProvider;
using Microsoft.AspNetCore.Http;

namespace Application.Services.UserProvider
{

    public class UserProvider : IUserProvider
    {
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppDbContext db;

        public UserProvider(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            this.httpContextAccessor = httpContextAccessor;
            db = dbContext;
            mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, CurrentUserDto>()).CreateMapper();
        }

        public Guid? GetCurrentUserId()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var userIdClaim = httpContext.User.FindFirst("userId");
            return userIdClaim != null
                ? Guid.Parse(userIdClaim.Value)
                : (Guid?)null;
        }

        public CurrentUserDto GetCurrentUser()
        {
            User user = db.Users.GetById(EnsureCurrentUserId());
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            CurrentUserDto dto = mapper.Map<CurrentUserDto>(user);
            dto.RoleType = db.Roles.GetById(user.RoleId).RoleType;
            dto.Language = GetCurrentUserLanguage();

            return dto;
        }
        
        public Guid EnsureCurrentUserId()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("Невозможно определить текущего пользователя");

            return userId.Value;
        }

        private string GetCurrentUserLanguage()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var langClaim = httpContext.User.FindFirst("lang");
            string lang = langClaim?.Value ?? "ru";
            return lang;
        }
    }
}