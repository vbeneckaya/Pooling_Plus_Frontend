using System.Collections.Generic;
using System.Linq;
using Application.Users;
using Microsoft.AspNetCore.Mvc;
//using Web.CustomAttributes;
using Application.Shared;
using Application.Extensions;

namespace Web.Controllers.API
{
    public class UsersController : Controller//Base
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }
        /// <summary>
        /// Список доступных пользователю колонок
        /// </summary>
        [HttpGet("users/list")]
        public IEnumerable<UserDto> GetList([FromBody]SearchForm form)
        {
            return usersService.GetAll(form);
        }

        public UserDto GetById([FromBody] string id)
        {
            var user = usersService.Get(id.ParseObjectId());
            return user.User;
        }

        public ValidateResult SaveOrCreate([FromBody] UserCreateUpdateFormDto userForm)
        {
            return usersService.SaveOrCreate(userForm);
        }

        public ValidateResult ChangeActivity([FromBody] string id)
        {
            return usersService.ChangeActivity(id);
        }
    }
}