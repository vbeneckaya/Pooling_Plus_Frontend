using Domain.Services.Users;

namespace Domain.Services.Roles
{
    public class RoleDto: IDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}