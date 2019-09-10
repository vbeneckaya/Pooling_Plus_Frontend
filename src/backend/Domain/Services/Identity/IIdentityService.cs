using Domain.Shared;

namespace Domain.Services.Identity
{
    public interface IIdentityService : IService
    {
        VerificationResultWith<TokenModel> GetToken(IdentityModel model);
        UserConfiguration GetConfiguration();
    }
}