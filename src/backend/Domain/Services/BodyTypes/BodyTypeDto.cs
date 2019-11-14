using Domain.Enums;
using Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.BodyTypes
{
    public class BodyTypeDto: IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
