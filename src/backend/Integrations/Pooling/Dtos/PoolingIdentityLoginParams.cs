namespace Integrations.Pooling.Dtos
{
    public class PoolingIdentityLoginParams
    {
        public PoolingIdentityLoginParams(string username ,string password)
        {
            Username = username; 
            Password = password; 
        }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class PoolingIdentityLoginResponse
    {
        public string accessToken { get; set; }
        public string accessTokenLifeTimeSeconds { get; set; }
        public string refreshToken { get; set; }
        public string refreshTokenLifeTimeSeconds { get; set; }
        public string userData { get; set; }
        public string permissions { get; set; }
    }
}