using Domain.Services.Permissions;
using System.Collections;
using System.Collections.Generic;

namespace Domain.Services.Roles
{
    public class RoleDto: IDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<PermissionInfo> Permissions { get; set; }

        public int UsersCount { get; set; }
    }
}