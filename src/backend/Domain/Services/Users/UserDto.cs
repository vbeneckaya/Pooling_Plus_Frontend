﻿using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.Users
{
    public class UserDto : IDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RoleId { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string FieldsConfig { get; set; }

        [FieldType(FieldType.Select, source: nameof(TransportCompanies))]
        public LookUpDto CarrierId { get; set; }
    }
}
