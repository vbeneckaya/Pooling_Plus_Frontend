using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.Users
{
    public class UserDto : IDto
    {
        public string Id { get; set; }
        
        [FieldType(FieldType.Text), IsRequired]
        public string UserName { get; set; }

        [FieldType(FieldType.Text), IsRequired]
        public string Email { get; set; }

        public string Role { get; set; }

        [FieldType(FieldType.Select), IsRequired]
        public LookUpDto RoleId { get; set; }

        [FieldType(FieldType.Password)]
        public string Password { get; set; }

        public bool IsActive { get; set; }

        public string FieldsConfig { get; set; }

        [FieldType(FieldType.Select, source: nameof(TransportCompanies))]
        public LookUpDto CarrierId { get; set; }

        [FieldType(FieldType.Select, source: nameof(Clients))]
        public LookUpDto ClientId { get; set; }
        
        [FieldType(FieldType.Select, source: nameof(Providers))]
        public LookUpDto ProviderId { get; set; }
        
        public bool IsEditable { get; set; }
    }
}
