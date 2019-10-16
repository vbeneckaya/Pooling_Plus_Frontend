using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared.FormFilters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterFieldAttribute: Attribute
    {
        public string Name { get; set; }

        public FilterFieldType Type { get; set; } = FilterFieldType.String;

        public bool Sorted { get; set; } = true;

        public bool Filtered { get; set; } = true;

        public bool Searched { get; set; } = true;
    }
}
