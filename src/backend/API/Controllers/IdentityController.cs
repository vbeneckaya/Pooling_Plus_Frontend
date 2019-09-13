using Domain.Services.Identity;
using Infrastructure.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService identityService;

        public IdentityController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }
        /// <summary>
        /// Получение информации о пользователе
        /// </summary>
        [HttpGet("identity/userInfo")] 
        public UserInfo Configuration()
        {
            return identityService.GetConfiguration();
        }
        
        /// <summary>
        /// Авторизация, получение токена для логина и пароля
        /// </summary>
        [HttpPost("identity/login")]
        public IActionResult Login([FromBody]IdentityModel model)
        {
            var identity = identityService.GetToken(model);

            switch (identity.Result)
            {
                case VerificationResult.Ok:
                    return Ok(identity.Data);

                case VerificationResult.Forbidden:
                    return BadRequest("UserNotFound");

                case VerificationResult.WrongCredentials:
                    return BadRequest("UserIncorrectData");

                default:
                    return StatusCode(500);
            }
        }
    }
}