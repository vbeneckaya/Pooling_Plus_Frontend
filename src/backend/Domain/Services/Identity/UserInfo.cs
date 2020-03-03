using Domain.Services.Roles;

namespace Domain.Services.Identity
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string UserRole { get; set; }

        public RoleDto Role { get; set; }
    }
}