using Domain.Enums;
using Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Tonnages
{
    public class TonnageDto : IDto
    {
        public string Id { get; set; }

        [FieldType(FieldType.Text), OrderNumber(1), IsRequired]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
