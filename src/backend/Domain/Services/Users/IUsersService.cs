using System;
using Domain.Persistables;
using Domain.Shared;

namespace Domain.Services.Users
{
    public interface IUsersService : IDictonaryService<User, UserDto>
    {
        ValidateResult SetActive(Guid id, bool active);
    }
}
