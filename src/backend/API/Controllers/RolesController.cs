using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Roles;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;

namespace API.Controllers
{
    [Route("api/roles")]
    public class RolesController : DictionaryController<IRolesService, Role, RoleDto>
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

        public RolesController(IRolesService rolesService) : base(rolesService)
        {
        }
    }
}