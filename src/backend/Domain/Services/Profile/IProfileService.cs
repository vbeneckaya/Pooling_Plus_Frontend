using Domain.Shared;

namespace Domain.Services.Profile
{
    public interface IProfileService : IService
    {
        ProfileDto GetProfile();
        ValidateResult Save(SaveProfileDto dto);
        ValidateResult SetNewPassword(SetNewPasswordDto dto);
    }
}