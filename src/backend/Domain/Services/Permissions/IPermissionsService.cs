using Domain.Enums;
using Domain.Persistables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Permissions
{
    public interface IPermissionsService
    {
        IEnumerable<RolePermission> GetPermissionsForRole(string roleName);

        IEnumerable<RolePermission> GetPermissionsForRole(Guid? roleId);

        IEnumerable<PermissionInfo> GetPermissionsInfoForRole(string roleName);

        IEnumerable<PermissionInfo> GetPermissionsInfoForRole(Guid? roleId);

        void UpdateRolePermissions(string roleName, IEnumerable<RolePermissions> permissions);
    }
}
