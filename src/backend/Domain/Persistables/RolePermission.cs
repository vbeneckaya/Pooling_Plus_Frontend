using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Persistables
{
    public class RolePermission : IPersistable
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public Role Role { get; set; }

        public RolePermissions PermissionCode { get; set; }
    }
}
