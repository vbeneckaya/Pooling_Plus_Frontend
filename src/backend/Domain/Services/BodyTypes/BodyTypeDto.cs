using Application.Shared.Excel.Columns;
using Domain.Enums;
using Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.BodyTypes
{
    public class BodyTypeDto: IDto
    {
        [ExcelIgnore]
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }

        [FieldType(FieldType.Boolean), OrderNumber(2)]
        public bool? IsActive { get; set; }
    }
}
