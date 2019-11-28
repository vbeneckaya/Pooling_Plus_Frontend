namespace Domain.Services.Profile
{
    public class SetNewPasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}