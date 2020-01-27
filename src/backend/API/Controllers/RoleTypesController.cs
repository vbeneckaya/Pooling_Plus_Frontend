using Domain.Enums;
using Domain.Services.Roles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/roleTypes")]
    public class RoleTypesController : EnumController<IRoleTypesService,RoleTypes>
    {
        public RoleTypesController(IRoleTypesService roleTypesService) : base(roleTypesService)
        {
        }
    }
}