using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.VehicleTypes
{
    public class VehicleTypeDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1)]
        public string Name { get; set; }
    }
}
