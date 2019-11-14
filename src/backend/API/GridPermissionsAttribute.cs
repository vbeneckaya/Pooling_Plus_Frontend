using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class GridPermissionsAttribute: Attribute
    {
        public RolePermissions Search { get; set; } = RolePermissions.None;

        public RolePermissions SaveOrCreate { get; set; } = RolePermissions.None;
    }
}
