using Domain.Services.AppConfiguration;
using Domain.Services.Profile;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/profile")]
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
        [HttpGet("info")] 
        public ProfileDto Info()
        {
            return profileService.GetProfile();
        }

        /// <summary>
        /// Сохранение новой информации о пользователе
        /// </summary>
        [HttpPost("save")] 
        public ValidateResult Save([FromBody]SaveProfileDto dto)
        {
            return profileService.Save(dto);
        }

        /// <summary>
        /// Задание нового пароля
        /// </summary>
        [HttpPost("setNewPassword")] 
        public ValidateResult SetNewPassword([FromBody]SetNewPasswordDto dto)
        {
            return profileService.SetNewPassword(dto);
        }
    }
}