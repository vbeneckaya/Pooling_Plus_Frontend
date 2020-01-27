using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.Clients
{
    public class ClientDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), IsRequired, OrderNumber(1)]
        public string Name { get; set; }

        public string PoolingId { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(3)]
        public bool? IsActive { get; set; }

        public bool IsEditable { get; set; }
    }
}
