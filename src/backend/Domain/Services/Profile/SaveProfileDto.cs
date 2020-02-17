namespace Domain.Services.Profile
{
    public class SaveProfileDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PoolingLogin { get; set; }
        public string PoolingPassword { get; set; }
        public string FmCPLogin;
        public string FmCPPassword;
    }
}