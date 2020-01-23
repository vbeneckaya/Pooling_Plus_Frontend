﻿using Domain.Enums;
using Domain.Extensions;
using Domain.Shared;

namespace Domain.Services.ProductTypes
{
    public class ProductTypeDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }

        public string PoolingId { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(3)]
        public bool? IsActive { get; set; }

        public bool IsEditable { get; set; }
    }
}
