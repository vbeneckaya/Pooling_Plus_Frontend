using Domain.Enums;
using Domain.Extensions;
using Domain.Services.Permissions;
using System.Collections.Generic;

namespace Domain.Services.Roles
{
    public class RoleDto: IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), IsRequired]
        public string Name { get; set; }

        public bool IsActive { get; set; }
        
        public RoleTypeInfo RoleType { get; set; }

        public IEnumerable<PermissionInfo> Permissions { get; set; }

        public IEnumerable<string> Actions { get; set; }

        public int UsersCount { get; set; }
        
        public bool IsEditable { get; set; }
    }
}