using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DAL;
using DAL.Queries;
using Domain.Enums;
using Domain.Services.Identity;
using Domain.Services.UserIdProvider;
using Domain.Shared;
using Infrastructure.Extensions;
using Infrastructure.Signing;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Identity
{
    
    public class IdentityService : IIdentityService
        {
            private readonly IUserIdProvider userIdProvider;
            private readonly AppDbContext db;

            public IdentityService(IUserIdProvider userIdProvider, AppDbContext dbContext)
        {
            this.userIdProvider = userIdProvider;
            db = dbContext;
        }

        public VerificationResultWith<TokenModel> GetToken(IdentityModel model)
        {
            var user = db.Users.GetByLogin(model.Login);

            if (user != null && !user.IsActive)
                return new VerificationResultWith<TokenModel>{Result = VerificationResult.Forbidden, Data = null};

            var identity = GetIdentity(model.Login, model.Password);

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

            return new VerificationResultWith<TokenModel>{Result = VerificationResult.Ok, Data = new TokenModel(encodedJwt, identity.Name, identity.FindFirst(ClaimsIdentity.DefaultRoleClaimType).Value)};
        }

        public UserConfiguration GetConfiguration()
        {
            var currentUserId = userIdProvider.GetCurrentUserId();
            UserConfiguration userConfiguration = new UserConfiguration
            {   Grids = new List<UserConfigurationGridItem>
                {
                    new UserConfigurationGridItem
                    {
                        Name = "Orders", 
                        CanCreateByForm = true, 
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn("Incoming", FiledType.Text)
                        }
                    },
                    new UserConfigurationGridItem
                    {
                        Name = "Transportations", 
                        CanCreateByForm = false, 
                        CanImportFromExcel = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn("From", FiledType.Text),
                            new UserConfigurationGridColumn("To", FiledType.Text)
                        }
                    },
                }, 
                Dictionaries = new List<UserConfigurationDictionaryItem>
                {
                    new UserConfigurationDictionaryItem{
                        Name = "Users", 
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn("Name", FiledType.Text),
                            new UserConfigurationGridColumn("Email", FiledType.Text),
                            new UserConfigurationGridColumn("Role", FiledType.Text),
                            new UserConfigurationGridColumn("IsActive", FiledType.Bool),
                        }
                        
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = "Roles",
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn("Name", FiledType.Text),
                        }
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = "Translations",
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn("Name", FiledType.Text),
                            new UserConfigurationGridColumn("Ru", FiledType.Text),
                            new UserConfigurationGridColumn("En", FiledType.Text),
                        }
                    },
                }
            };
            if (currentUserId != null)
            {
                var user = db.Users.GetById(currentUserId.Value);
                if (user != null)
                {
                    //TODO Строить исходя из Роли
                }
            }
            return userConfiguration;
        }

        private ClaimsIdentity GetIdentity(string userName, string password)
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
                new Claim("userId", user.Id.ToString())
            };

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }     
}