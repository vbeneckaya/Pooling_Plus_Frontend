using System;

namespace Domain.Services.Identity
{
    public class IdentityByTokenModel
    {
        public Guid UserId { get; set; }

        public string Token { get; set; }
        
        public string Language { get; set; } = "ru";
    }
}