using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.Permissions;
using Domain.Shared;

namespace Domain.Services.Roles
{
    public interface IRolesService : IDictonaryService<Role, RoleDto>
    {
        ValidateResult SetActive(Guid id, bool active);

        ValidateResult SetPermissions(Guid roleId, IEnumerable<RolePermissions> permissions);

        IEnumerable<PermissionInfo> GetAllPermissions();

        RoleDto MapFromEntityToDto(Role entity);
    }
}