using System.Linq;
using DAL.Queries;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Profile;
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
            var user = dataService.GetDbSet<User>().GetById(currentUserId.Value);
            return new ProfileDto
            {
                Email = user.Email,
                UserName = user.Name,
                RoleName = user.Role.Name,
            };
        }

        public ValidateResult Save(SaveProfileDto dto)
        {
            var currentUserId = userProvider.GetCurrentUserId();
            var user = dataService.GetDbSet<User>().GetById(currentUserId.Value);
            
            if (string.IsNullOrEmpty(dto.Email))
                return new ValidateResult
                {
                    Error = "userEmailIsEmpty"
                };

            if (!IsValidEmail(dto.Email))
                return new ValidateResult("userEmailIsInvalid");

            if (dataService.GetDbSet<User>().Any(x => x.Email == dto.Email && x.Id != user.Id))
                return new ValidateResult("userEmailIsNotAvailable");

            if (string.IsNullOrEmpty(dto.UserName))
                return new ValidateResult("userNameIsEmpty");
            
            user.Email = dto.Email;
            user.Name = dto.UserName;
            
            dataService.SaveChanges();
            
            return new ValidateResult
            {
                ResultType = ValidateResultType.Updated,
                Error = null
            };
        }

        public ValidateResult SetNewPassword(SetNewPasswordDto dto)
        {
            var currentUserId = userProvider.GetCurrentUserId();
            var user = dataService.GetDbSet<User>().GetById(currentUserId.Value);

            if (user.PasswordHash == dto.OldPassword.GetHash())
            {
                if(string.IsNullOrEmpty(dto.NewPassword))
                    return new ValidateResult("epmtyPassword");
                
                if(dto.NewPassword.Length < 6)
                    return new ValidateResult("shortPassword");
                
                user.PasswordHash = dto.NewPassword.GetHash();

                dataService.SaveChanges();
                return new ValidateResult();
            }

            return new ValidateResult();
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