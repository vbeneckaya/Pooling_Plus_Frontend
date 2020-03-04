using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Roles;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/roles")]
    [ApiExplorerSettings(IgnoreApi=true)]
    public class RolesController : DictionaryController<IRolesService, Role, RoleDto>
    {
        public RolesController(IRolesService rolesService) : base(rolesService)
        {
        }

        /// <summary>
        /// Получение данных для выпадающего списка в 
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

       
        /// <summary>
        /// Получение списка всех доступных разрешений
        /// </summary>
        [HttpGet("allPermissions")]
        public IActionResult GetAllPermissions()
        {
            try
            {
                var result = _service.GetAllPermissions();

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to get permissions list");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Получение списка всех доступных действий
        /// </summary>
        [HttpGet("allActions")]
        public IActionResult GetAllActions()
        {
            try
            {
                var result = _service.GetAllActions();

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to get actions list");
                return StatusCode(500);
            }
        }
        
        
        /// <summary>
        /// Получение списка всех доступных видов компаний для роли
        /// </summary>
        [HttpGet("getCompanyTypeByRole/{id}")]
        public IActionResult GetCompanyTypeByRole([FromRoute] Guid id)
        {
            try
            {
                var result = _service.GetCompanyTypeByRole(id);

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to get company type by role id");
                return StatusCode(500);
            }
        }
    }
}