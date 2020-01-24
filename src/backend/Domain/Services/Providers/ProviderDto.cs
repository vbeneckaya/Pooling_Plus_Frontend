using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.Providers
{
    public class ProviderDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(2), IsRequired]
        public string Inn { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(3), IsRequired]
        public string Cpp { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(4), IsRequired]
        public string LegalAddress { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(5), IsRequired]
        public string ActualAddress { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(6), IsRequired]
        public string ContactPerson { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(7), IsRequired]
        public string ContactPhone { get; set; } 
        
        [FieldType(FieldType.Text), OrderNumber(8), IsRequired]
        public string Email { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(9)]
        public bool? IsActive { get; set; }

        public bool IsEditable { get; set; }
    }
}
