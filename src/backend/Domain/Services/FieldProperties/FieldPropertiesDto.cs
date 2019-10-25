using System.Collections.Generic;

namespace Domain.Services.FieldProperties
{
    public class FieldPropertiesDto
    {
        public IEnumerable<FieldForFieldProperties> Fields { get; set; }
        public string CompanyId { get; set; }
        public string RoleId { get; set; }
        public string ForEntity { get; set; }        
    }
}