namespace Application.Users
{
    public class UserCreateUpdateFormDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleId { get; set; }
        public bool IsActive { get; set; }
        public string[] Companies { get; set; }
        public string[] Platforms { get; set; }
        public string[] Activities { get; set; }
    }
}
