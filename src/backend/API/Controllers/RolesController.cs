using Domain.Persistables;
using Domain.Services.Roles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("Roles")]
    public class RolesController : DictonaryController<IRolesService, Role, RoleDto>
    {
        public RolesController(IRolesService rolesService) : base(rolesService)
        {
        }
    }
}