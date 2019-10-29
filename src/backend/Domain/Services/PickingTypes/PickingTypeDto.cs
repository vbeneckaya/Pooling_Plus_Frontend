using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.PickingTypes
{
    public class PickingTypeDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1)]
        public string Name { get; set; }
    }
}