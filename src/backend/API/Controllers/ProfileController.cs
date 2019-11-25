using Domain.Services.AppConfiguration;
using Domain.Services.Profile;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api")]
    public class ProfileController : Controller
    {
        private readonly IProfileService profileService;

        public ProfileController(IProfileService profileService)
        {
            this.profileService = profileService;
        }

        /// <summary>
        /// Получение профиля текущего пользователя
        /// </summary>
        [HttpGet("profile")] 
        public ProfileDto Profile()
        {
            return profileService.GetProfile();
        }

        /// <summary>
        /// Сохранение новой информации о пользователе
        /// </summary>
        [HttpGet("save")] 
        public ValidateResult Save(SaveProfileDto dto)
        {
            return profileService.Save(dto);
        }

        /// <summary>
        /// Задание нового пароля
        /// </summary>
        [HttpGet("setNewPassword")] 
        public ValidateResult SetNewPassword(SetNewPasswordDto dto)
        {
            return profileService.SetNewPassword(dto);
        }
    }
}