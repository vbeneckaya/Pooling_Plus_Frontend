using System;

namespace Domain.Services.UserProvider
{
    public class CurrentUserDto
    {
        public Guid? Id { get; set; }

        public Guid? RoleId { get; set; }

        public string Name { get; set; }

        public string Language { get; set; }

        public Guid? CompanyId { get; set; }
    }
}
