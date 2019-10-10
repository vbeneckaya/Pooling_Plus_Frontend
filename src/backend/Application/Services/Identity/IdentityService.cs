using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DAL;
using DAL.Queries;
using Domain.Services.Identity;
using Domain.Services.UserProvider;
using Domain.Extensions;
using Domain.Shared;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Identity
{
    
    public class IdentityService : IIdentityService
        {
            private readonly IUserProvider userIdProvider;
            private readonly AppDbContext db;

            public IdentityService(IUserProvider userIdProvider, AppDbContext dbContext)
        {
            this.userIdProvider = userIdProvider;
            db = dbContext;
        }

        public VerificationResultWith<TokenModel> GetToken(IdentityModel model)
        {
            var user = db.Users.GetByLogin(model.Login);

            if (user != null && !user.IsActive)
                return new VerificationResultWith<TokenModel>{Result = VerificationResult.Forbidden, Data = null};

            var identity = GetIdentity(model.Login, model.Password, model.Language);

            if (identity == null)
                return new VerificationResultWith<TokenModel>{Result = VerificationResult.WrongCredentials, Data = null};

            var now = DateTime.Now;

            // Creating JWT
            var jwt = new JwtSecurityToken(
                issuer: SigningOptions.SignIssuer,
                audience: SigningOptions.SignAudience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.AddDays(7),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SigningOptions.SignKey)), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new VerificationResultWith<TokenModel>{Result = VerificationResult.Ok, Data = new TokenModel(encodedJwt)};
        }

        public UserInfo GetUserInfo()
        {
            var user = userIdProvider.GetCurrentUser();

            //TODO Получать имя пользователя и роль
            return new UserInfo
            {   
                UserName = user.Name,
                UserRole = user.RoleId.HasValue ? db.Roles.GetById(user.RoleId.Value)?.Name : null
            };
        }

        private ClaimsIdentity GetIdentity(string userName, string password, string language)
        {
            var user = db.Users.GetByLogin(userName);
            if (user == null)
                return null;

            var role = db.Roles.GetById(user.RoleId);
            if (role == null)
                return null;

            var passwordHash = password.GetHash();
            if (passwordHash != user.PasswordHash)
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name),
                new Claim("userId", user.Id.ToString()),
                new Claim("lang", language)
            };

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }     
}