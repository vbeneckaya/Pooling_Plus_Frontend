using System.Linq;
using DAL.Queries;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Profile;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;

namespace Application.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly IUserProvider userProvider;
        private readonly ICommonDataService dataService;

        public ProfileService(IUserProvider userProvider, ICommonDataService dataService)
        {
            this.userProvider = userProvider;
            this.dataService = dataService;
        }
        public ProfileDto GetProfile()
        {
            var currentUserId = userProvider.GetCurrentUserId();
            var user = dataService
                .GetDbSet<User>()
                .GetById(currentUserId.Value);

            var role = dataService
                .GetDbSet<Role>()
                .GetById(user.RoleId);
            
            return new ProfileDto
            {
                Email = user.Email,
                UserName = user.Name,
                RoleName = role.Name,
            };
        }

        public ValidateResult Save(SaveProfileDto dto)
        {
            var currentUserId = userProvider.GetCurrentUserId();
            var user = dataService.GetDbSet<User>().GetById(currentUserId.Value);
            
            var lang = userProvider.GetCurrentUser().Language;
            
            var result = new DetailedValidationResult();
            
            if (string.IsNullOrEmpty(dto.Email))
                result.AddError(nameof(dto.Email), "userEmailIsEmpty".Translate(lang), ValidationErrorType.ValueIsRequired); 

            if (!string.IsNullOrEmpty(dto.Email) && !IsValidEmail(dto.Email))
                result.AddError(nameof(dto.Email), "userEmailIsInvalid".Translate(lang), ValidationErrorType.InvalidValueFormat);

            if (dataService.GetDbSet<User>().Any(x => x.Email == dto.Email && x.Id != user.Id))
                result.AddError(nameof(dto.Email), "userEmailIsNotAvailable".Translate(lang), ValidationErrorType.DuplicatedRecord);

            if (string.IsNullOrEmpty(dto.UserName))
                result.AddError(nameof(dto.UserName), "userNameIsEmpty".Translate(lang), ValidationErrorType.ValueIsRequired);

            if (!result.IsError)
            {
                user.Email = dto.Email;
                user.Name = dto.UserName;
            
                dataService.SaveChanges();
            }
            
            return result;
        }

        public ValidateResult SetNewPassword(SetNewPasswordDto dto)
        {
            var currentUserId = userProvider.GetCurrentUserId();
            var user = dataService.GetDbSet<User>().GetById(currentUserId.Value);
            var lang = userProvider.GetCurrentUser().Language;
            
            var result = new DetailedValidationResult();

            if (user.PasswordHash == dto.OldPassword.GetHash())
            {
                if (string.IsNullOrEmpty(dto.NewPassword))
                {
                    result.AddError(nameof(dto.NewPassword), "users.emptyPassword".Translate(lang), ValidationErrorType.InvalidValueFormat);
                    return result;
                }

                if (dto.NewPassword.Length < 6)
                {
                    result.AddError(nameof(dto.NewPassword), "shortPassword".Translate(lang), ValidationErrorType.InvalidValueFormat);
                    return result;
                }

                user.PasswordHash = dto.NewPassword.GetHash();

                dataService.SaveChanges();
            }
            else
                result.AddError(nameof(dto.OldPassword), "wrongOldPassword".Translate(lang), ValidationErrorType.ValueIsRequired);

            return result;
        }
        
        bool IsValidEmail(string email)
        {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }
    }
}