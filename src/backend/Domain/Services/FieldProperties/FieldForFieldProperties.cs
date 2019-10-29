using System.Collections.Generic;

namespace Domain.Services.FieldProperties
{
    public class FieldForFieldProperties
    {
        public string FieldName { get; set; }
        public Dictionary<string, string> AccessTypes { get; set; }
    }
}