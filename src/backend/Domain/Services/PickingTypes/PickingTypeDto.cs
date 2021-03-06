using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.PickingTypes
{
    public class PickingTypeDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(3)]
        public bool? IsActive { get; set; } = true;

        public bool IsEditable { get; set; }
    }
}