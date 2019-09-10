using Domain.Persistables;

namespace Domain.Services.Users
{
    public interface IUsersService : IDictonaryService<User, UserDto>
    {
    }
}
