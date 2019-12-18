using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.TransportCompanies
{
    public class TransportCompanyDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Title { get; set; }

        [FieldType(FieldType.Text), OrderNumber(2)]
        public string ContractNumber { get; set; }

        [FieldType(FieldType.Text), OrderNumber(3)]
        public string DateOfPowerOfAttorney { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(4)]
        public bool? IsActive { get; set; }

        public bool IsEditable { get; set; }
    }
}