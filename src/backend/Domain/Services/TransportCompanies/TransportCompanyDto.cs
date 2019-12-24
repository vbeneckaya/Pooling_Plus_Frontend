using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.TransportCompanies
{
    public class TransportCompanyDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Title { get; set; }

        [FieldType(FieldType.Select, source: nameof(Companies)), OrderNumber(2)]
        public LookUpDto CompanyId { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(3)]
        public bool? IsActive { get; set; }

        public bool IsEditable { get; set; }
    }
}