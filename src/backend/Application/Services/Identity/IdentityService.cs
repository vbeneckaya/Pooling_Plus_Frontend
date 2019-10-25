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
using System.Linq;
using DAL.Services;
using Domain.Persistables;
using Domain.Enums;
using Domain.Services.Permissions;
using Domain.Services.Translations;
using Domain.Services.Roles;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Identity
{
    
    public class IdentityService : IIdentityService
    {
            
        private readonly IUserProvider _userIdProvider;
        
        private readonly ICommonDataService _dataService;

        private readonly ITranslationsService _translationsService;

        private readonly IRolesService _rolesService;

        public IdentityService(IUserProvider userIdProvider, ICommonDataService dataService, IRolesService rolesService, ITranslationsService translationsService)
        {
            this._userIdProvider = userIdProvider;
            this._dataService = dataService;
            this._rolesService = rolesService;
            this._translationsService = translationsService;
        }

        public VerificationResultWith<TokenModel> GetToken(IdentityModel model)
        {
            var user = this._dataService.GetDbSet<User>().GetByLogin(model.Login);

            if (user != null && !user.IsActive)
                return new VerificationResultWith<TokenModel> { Result = VerificationResult.Forbidden, Data = null };

            var identity = GetIdentity(model.Login, model.Password, model.Language);

            if (identity == null)
                return new VerificationResultWith<TokenModel> { Result = VerificationResult.WrongCredentials, Data = null };

            var now = DateTime.Now;

            var claims = identity.Claims;

            var role = this._dataService.GetDbSet<Role>().GetById(user.RoleId);

            if (role?.Permissions != null)
            {
                var userPermissions = role
                .Permissions
                .Cast<RolePermissions>()
                .Select(i => new Claim("Permission", i.GetPermissionName()));

                claims = claims.Union(userPermissions);
            }

            // Creating JWT
            var jwt = new JwtSecurityToken(
                issuer: SigningOptions.SignIssuer,
                audience: SigningOptions.SignAudience,
                notBefore: now,
                claims: claims,
                expires: now.AddDays(7),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SigningOptions.SignKey)), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new VerificationResultWith<TokenModel>{Result = VerificationResult.Ok, Data = new TokenModel(encodedJwt)};
        }

        public UserInfo GetUserInfo()
        {
            var user = _userIdProvider.GetCurrentUser();
            var role = user.RoleId.HasValue ? this._dataService.GetDbSet<Role>().GetById(user.RoleId.Value) : null;

            //TODO Получать имя пользователя и роль
            return new UserInfo
            {   
                UserName = user.Name,
                UserRole = user.RoleId.HasValue ? _dataService.GetDbSet<Role>().GetById(user.RoleId.Value)?.Name : null,
                Role = role != null ? _rolesService.MapFromEntityToDto(role) : null
            };
        }

        private ClaimsIdentity GetIdentity(string userName, string password, string language)
        {
            var user = _dataService.GetDbSet<User>().GetByLogin(userName);
            if (user == null)
                return null;

            var role = _dataService.GetDbSet<Role>().GetById(user.RoleId);
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

        public bool HasPermissions(User user, RolePermissions permission)
        {
            return user?.Role?.Permissions
                ?.Cast<RolePermissions>()
                ?.Any(i => i == permission || i == RolePermissions.Admin) ?? false;
        }


        public bool HasPermissions(RolePermissions permission)
        {
            var userId = this._userIdProvider.GetCurrentUserId();

            if (!userId.HasValue) return false;

            var user = _dataService
                .GetDbSet<User>()
                .Include(i => i.Role)
                .First(i => i.Id == userId);

            return HasPermissions(user, permission);
        }
    }
}