using System;
using Domain.Persistables;
using Domain.Shared;

namespace Domain.Services.Roles
{
    public interface IRolesService : IDictonaryService<Role, RoleDto>
    {
        ValidateResult SetActive(Guid id, bool active);
    }
}