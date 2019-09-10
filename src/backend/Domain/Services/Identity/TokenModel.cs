namespace Domain.Services.Identity
{
    public class TokenModel
    {
        public TokenModel()
        {
        }

        public TokenModel(string token, string name, string role)
        {
            AccessToken = token;
            UserName = name;
            UserRole = role;
        }

        public string AccessToken { get; set; }

        public string UserName { get; set; }

        public string UserRole { get; set; }
    }
}