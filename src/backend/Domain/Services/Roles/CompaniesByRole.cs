using Domain.Enums;
using Domain.Extensions;
using Domain.Services.Permissions;
using System.Collections.Generic;

namespace Domain.Services.Roles
{
    public class CompaniesByRole
    {
        [FieldType(FieldType.Text), IsRequired]
        public string Field { get; set; }
        
        [FieldType(FieldType.Text), IsRequired]
        public string Source { get; set; }
    }
}