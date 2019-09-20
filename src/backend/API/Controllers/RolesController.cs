using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Roles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/roles")]
    public class RolesController : DictionaryController<IRolesService, Role, RoleDto>
    {
        public RolesController(IRolesService rolesService) : base(rolesService)
        {
        }
    }
}