using System.Linq;
using DAL.Queries;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.Profile;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;

namespace Application.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly IUserProvider _userProvider;
        private readonly ICommonDataService _dataService;

        private readonly IValidationService _validationService;

        public ProfileService(IUserProvider userProvider, ICommonDataService dataService, IValidationService validationService)
        {
            this._userProvider = userProvider;
            this._dataService = dataService;
            this._validationService = validationService;
        }
        public ProfileDto GetProfile()
        {
            var currentUserId = _userProvider.GetCurrentUserId();
            var user = _dataService
                .GetDbSet<User>()
                .GetById(currentUserId.Value);

            var role = _dataService
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
            var currentUserId = _userProvider.GetCurrentUserId();
            var user = _dataService.GetDbSet<User>().GetById(currentUserId.Value);
            
            var lang = _userProvider.GetCurrentUser().Language;

            var result = this._validationService.Validate(dto);
            
            if (string.IsNullOrEmpty(dto.Email))
                result.AddError(nameof(dto.Email), "userEmailIsEmpty".Translate(lang), ValidationErrorType.ValueIsRequired); 

            if (!string.IsNullOrEmpty(dto.Email) && !IsValidEmail(dto.Email))
                result.AddError(nameof(dto.Email), "userEmailIsInvalid".Translate(lang), ValidationErrorType.InvalidValueFormat);

            if (_dataService.GetDbSet<User>().Any(x => x.Email == dto.Email && x.Id != user.Id))
                result.AddError(nameof(dto.Email), "User.DuplicatedRecord".Translate(lang), ValidationErrorType.DuplicatedRecord);

            if (string.IsNullOrEmpty(dto.UserName))
                result.AddError(nameof(dto.UserName), "userNameIsEmpty".Translate(lang), ValidationErrorType.ValueIsRequired);

            if (!result.IsError)
            {
                user.Email = dto.Email;
                user.Name = dto.UserName;
            
                _dataService.SaveChanges();
            }
            
            return result;
        }

        public ValidateResult SetNewPassword(SetNewPasswordDto dto)
        {
            var currentUserId = _userProvider.GetCurrentUserId();
            var user = _dataService.GetDbSet<User>().GetById(currentUserId.Value);
            var lang = _userProvider.GetCurrentUser().Language;
            
            var result = this._validationService.Validate(dto);

            if (!string.IsNullOrEmpty(dto.OldPassword) && user.PasswordHash != dto.OldPassword.GetHash())
            {
                result.AddError(nameof(dto.OldPassword), "wrongOldPassword".Translate(lang), ValidationErrorType.ValueIsRequired);
            }

            if (!result.IsError)
            {
                user.PasswordHash = dto.NewPassword.GetHash();
                _dataService.SaveChanges();
            }

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