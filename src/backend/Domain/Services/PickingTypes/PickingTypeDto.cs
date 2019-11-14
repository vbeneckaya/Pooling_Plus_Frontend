using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.PickingTypes
{
    public class PickingTypeDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }
    }
}