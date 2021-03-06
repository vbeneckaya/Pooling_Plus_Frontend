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

        public IdentityService(IUserProvider userIdProvider, ICommonDataService dataService)
        {
            _userIdProvider = userIdProvider;
            _dataService = dataService;
        }

        public VerificationResultWith<TokenModel> GetToken(IdentityModel model)
        {
            prepareIdentityModelBeforeLogin(model);

            var user = _dataService.GetDbSet<User>().GetByLogin(model.Login);

            if (user != null && !user.IsActive)
                return new VerificationResultWith<TokenModel> {Result = VerificationResult.Forbidden, Data = null};

            var identity = GetIdentity(model.Login, model.Password, model.Language);
            
            return GetTokenInner(user, identity);
        }

        public VerificationResultWith<TokenModel> GetToken(IdentityByTokenModel model)
        {
            var user = _dataService.GetById<User>(model.UserId);

            if (user != null && !user.IsActive)
                return new VerificationResultWith<TokenModel> {Result = VerificationResult.Forbidden, Data = null};

            var identity = GetIdentity(user.Email, Uri.UnescapeDataString(model.Token), model.Language, true);

            return GetTokenInner(user, identity);
        }

        public UserInfo GetUserInfo()
        {
            var user = _userIdProvider.GetCurrentUser();
            var role = user.RoleId.HasValue ? this._dataService.GetDbSet<Role>().GetById(user.RoleId.Value) : null;

            //TODO Получать имя пользователя и роль
            return new UserInfo
            {
                UserName = user.Name,
                UserRole = role?.Name,
                Role = role != null
                    ? new RoleDto
                    {
                        Id = role.Id.ToString(),
                        Name = role.Name,
                        IsActive = role.IsActive,
                        Actions = role.Actions,
                        Permissions = role?.Permissions?.Cast<RolePermissions>()?.Select(i => new PermissionInfo
                        {
                            Code = i,
                            Name = i.GetPermissionName()
                        }),
                        UsersCount = _dataService.GetDbSet<User>().Where(i => i.RoleId == role.Id).Count(),
                    }
                    : null
            };
        }

        private VerificationResultWith<TokenModel> GetTokenInner(User user, ClaimsIdentity identity)
        {
            if (identity == null)
                return new VerificationResultWith<TokenModel>
                    {Result = VerificationResult.WrongCredentials, Data = null};

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
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SigningOptions.SignKey)),
                    SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new VerificationResultWith<TokenModel>
                {Result = VerificationResult.Ok, Data = new TokenModel(encodedJwt)};
        }

        private ClaimsIdentity GetIdentity(string userName, string password, string language, bool byToken = false)
        {
            var user = _dataService.GetDbSet<User>().GetByLogin(userName);
            if (user == null)
                return null;

            var role = _dataService.GetDbSet<Role>().GetById(user.RoleId);
            if (role == null)
                return null;

            if (byToken)
            {
                var token = user.PasswordHash.GetHash();
                if (token != password)
                    return null;
            }
            else
            {
                var passwordHash = password.GetHash();
                if (passwordHash != user.PasswordHash)
                    return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name),
                new Claim("userId", user.Id.ToString()),
                new Claim("lang", language)
            };

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        }

        public bool HasPermissions(User user, RolePermissions permission)
        {
            return user?.Role?.Permissions
                       ?.Cast<RolePermissions>()
                       ?.Any(i => i == permission) ?? false;
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

        private void prepareIdentityModelBeforeLogin(IdentityModel model)
        {
            model.Login = model.Login.ToLower();
        }
    }
}