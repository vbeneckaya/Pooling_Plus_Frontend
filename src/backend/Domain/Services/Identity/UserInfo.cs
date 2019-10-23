using Domain.Services.Permissions;
using System.Collections.Generic;

namespace Domain.Services.Identity
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string UserRole { get; set; }

        public IEnumerable<PermissionInfo> Permissions { get; set; }
    }
}