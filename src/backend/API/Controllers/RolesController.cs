using API.Controllers.Shared;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.Roles;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;

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

        [HttpPost("setRolePermissions")]
        public IActionResult SetRolePermissions(Guid roleId, RolePermissions[] permissions)
        {
            try
            {
                _service.SetPermissions(roleId, permissions);

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update permissions for role");
                return StatusCode(500, ex.Message);
            }
        }

        public RolesController(IRolesService rolesService) : base(rolesService)
        {
        }
    }
}