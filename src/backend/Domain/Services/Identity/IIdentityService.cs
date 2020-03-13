using Domain.Enums;
using Domain.Persistables;
using Domain.Shared;

namespace Domain.Services.Identity
{
    public interface IIdentityService : IService
    {
        VerificationResultWith<TokenModel> GetToken(IdentityModel model);
        
        VerificationResultWith<TokenModel> GetToken(IdentityByTokenModel model);
        
        UserInfo GetUserInfo();

        bool HasPermissions(User user, RolePermissions permission);

        bool HasPermissions(RolePermissions permission);
    }
}