using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared.FormFilters
{
    public enum FilterFieldType
    {
        String,
        Integer,
        Decimal,
        DateRange,
        Options,
        Enum,
        Boolean
    }
}
