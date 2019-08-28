namespace Application.Users
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RoleId { get; set; }
        public string CurrentPlatform { get; set; }
        public string CurrentPlatformId { get; set; }
        public bool IsActive { get; set; }
        public string FieldsConfig { get; set; }
        public string[] Companies { get; set; }
        public string[] CompanyNames { get; set; }
        public string[] Platforms { get; set; }
        public string[] PlatformNames { get; set; }
        public string[] Activities { get; set; }
        public string[] ActivityNames { get; set; }
    }
}
