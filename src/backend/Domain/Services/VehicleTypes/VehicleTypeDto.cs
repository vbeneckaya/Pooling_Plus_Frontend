﻿using Application.Shared.Excel.Columns;
using Domain.Enums;
using Domain.Extensions;

namespace Domain.Services.VehicleTypes
{
    public class VehicleTypeDto : IDto
    {
        [ExcelIgnore]
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1)]
        public string Name { get; set; }

        [FieldType(FieldType.Select, source: nameof(Tonnages)), OrderNumber(2)]
        public string TonnageId { get; set; }

        [FieldType(FieldType.Select, source: nameof(BodyTypes)), OrderNumber(3)]
        public string BodyTypeId { get; set; }

        [FieldType(FieldType.Number), OrderNumber(4)]
        public string PalletsCount { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(5)]
        public bool? IsActive { get; set; }
    }
}
