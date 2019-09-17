using API.Controllers.Shared;
using Domain.Persistables;
using Domain.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/users")]
    public class UsersController : DictonaryController<IUsersService, User, UserDto>
    {
        public UsersController(IUsersService usersService) : base(usersService)
        {
        }
    }
}