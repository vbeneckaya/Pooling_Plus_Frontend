using System;
using Domain.Enums;

namespace Domain.Services.UserProvider
{
    public class CurrentUserDto
    {
        public Guid? Id { get; set; }

        public Guid? RoleId { get; set; }

        public string Name { get; set; }

        public string Language { get; set; }

        public Guid? CarrierId { get; set; }
        
        public Guid? ClientId { get; set; }
        
        public Guid? ProviderId { get; set; }

        public RoleTypes RoleType { get; set; }
    }
}
