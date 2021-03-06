﻿using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.DocumentTypes
{
    public class DocumentTypeDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(4)]
        public bool? IsActive { get; set; } = true;

        public bool IsEditable { get; set; }
    }
}
