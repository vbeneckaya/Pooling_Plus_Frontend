using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.Orders
{
    public class OrderItemDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text)]
        public string Nart { get; set; }

        [FieldType(FieldType.Text)]
        public string Description { get; set; }

        [FieldType(FieldType.Text)]
        public string CountryOfOrigin { get; set; }

        [FieldType(FieldType.Text)]
        public string Spgr { get; set; }

        [FieldType(FieldType.Text)]
        public string Ean { get; set; }

        [FieldType(FieldType.Number)]
        public int? ShelfLife { get; set; }

        [FieldType(FieldType.Number)]
        public int? Quantity { get; set; }
    }
}
