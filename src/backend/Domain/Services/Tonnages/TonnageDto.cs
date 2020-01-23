using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.Tonnages
{
    public class TonnageDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(2)]
        public bool? IsActive { get; set; }

        public bool IsEditable { get; set; }
    }
}
