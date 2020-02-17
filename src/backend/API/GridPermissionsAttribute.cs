using Domain.Enums;
using System;

namespace API
{
    public class GridPermissionsAttribute: Attribute
    {
        public RolePermissions Search { get; set; } = RolePermissions.None;

        public RolePermissions SaveOrCreate { get; set; } = RolePermissions.None;
    }
}
