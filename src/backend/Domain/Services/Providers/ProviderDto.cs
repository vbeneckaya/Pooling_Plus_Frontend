using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.Providers
{
    public class ProviderDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(2), IsRequired, MaxLength(12)]
        public string Inn { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(3), IsRequired, MaxLength(10)]
        public string Cpp { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(4), IsRequired, MaxLength(250)]
        public string LegalAddress { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(5), IsRequired, MaxLength(250)]
        public string ActualAddress { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(6), IsRequired, MaxLength(300)]
        public string ContactPerson { get; set; }
        
        [FieldType(FieldType.Text), OrderNumber(7), IsRequired, MaxLength(16)]
        public string ContactPhone { get; set; } 
        
        [FieldType(FieldType.Text), OrderNumber(8), IsRequired, MaxLength(70)]
        public string Email { get; set; }

        [FieldType(FieldType.Text), OrderNumber(9), MaxLength(70)]
        public string ReportId { get; set; }

        [FieldType(FieldType.Text), OrderNumber(10), MaxLength(70)]
        public string ReportPageNameForMobile { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(11)]
        public bool? IsActive { get; set; } = true;
        
        [FieldType(FieldType.Boolean), OrderNumber(12)]
        public bool IsPoolingIntegrated { get; set; }

        public bool IsEditable { get; set; }
    }
}
