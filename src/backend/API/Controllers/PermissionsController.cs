using Domain.Enums;
using Domain.Services.Identity;
using Domain.Services.Permissions;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/permissions")]

    public class PermissionsController: Controller
    {
        private readonly IIdentityService _identityService;

        private readonly IPermissionsService _permissionsService;

        public PermissionsController(IIdentityService identityService, IPermissionsService permissionsService)
        {
            this._identityService = identityService;
            this._permissionsService = permissionsService;
        }

        [HttpGet("getUserPermissions")]
        public IActionResult GetUserPermissions()
        {
            try
            {
                UserInfo userInfo = _identityService.GetUserInfo();

                var permissions = _permissionsService.GetPermissionsInfoForRole(userInfo.UserRole);

                return Ok(permissions);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to Get permissions for Role");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("updateRolePermissions")]
        public IActionResult UpdateRolePermissions(string roleName, IEnumerable<RolePermissions> permissions)
        {
            try
            {
                _permissionsService.UpdateRolePermissions(roleName, permissions);

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
    }
}
