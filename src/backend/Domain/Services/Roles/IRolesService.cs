using Domain.Persistables;
using Domain.Services.Users;

namespace Domain.Services.Roles
{
    public interface IRolesService : IDictonaryService<Role, RoleDto>
    {
    }
}