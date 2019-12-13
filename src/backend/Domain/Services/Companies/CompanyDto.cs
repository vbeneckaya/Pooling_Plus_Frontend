using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.Companies
{
    public class CompanyDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(2)]
        public bool? IsActive { get; set; }
    }
}
