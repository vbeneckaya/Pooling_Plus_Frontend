using Domain.Enums;
using System;

namespace Domain.Extensions
{
    public class FieldTypeAttribute : Attribute
    {
        public FieldType Type { get; set; }
        public string Source { get; set; }
        public bool ShowRawValue { get; set; }

        public FieldTypeAttribute(FieldType type, string source = null, bool showRawValue = false)
        {
            Type = type;
            Source = source;
            ShowRawValue = showRawValue;
        }
    }
}
