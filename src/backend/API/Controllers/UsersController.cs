using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;

namespace API.Controllers
{
    [Route("api/users")]
    [ApiExplorerSettings(IgnoreApi=true)]
    public class UsersController : DictionaryController<IUsersService, User, UserDto>
    {
        /// <summary>
        /// ѕолучение данных дл€ выпадающего списка в 
        /// </summary>
        [HttpPost("setActive/{id}/{active}")]
        public IActionResult SetActive(Guid id, bool active)
        {
            try
            {
                var result = _service.SetActive(id, active);

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to Change active for User");
                return StatusCode(500);
            }
        }

        public UsersController(IUsersService usersService) : base(usersService)
        {
        }
    }
}