using System.Collections.Generic;

namespace Domain.Services.FieldProperties
{
    public class FieldForFieldProperties
    {
        public string Name { get; set; }
        public string From { get; set; }
        public string FromType { get; set; }
        public IEnumerable<FieldPropertiesAccessTypeDto> FieldPropertiesAccessTypes { get; set; }
    }

    public class FieldPropertiesAccessTypeDto
    {
        public string State { get; set; }
        public string AccessType { get; set; }
        
        public string CompanyId { get; set; }

        public int CompareTo(object o)
        {
            FieldPropertiesAccessTypeDto nextItem = o as FieldPropertiesAccessTypeDto;
            if (nextItem != null)
                return GetPoints(nextItem) - GetPoints(this);
            else
                return -1;
        }

        private int GetPoints(FieldPropertiesAccessTypeDto item)
        {
            if (item.AccessType == "Hidden") return 1;
            if (item.AccessType == "Show") return 2;
            if (item.AccessType == "Edit") return 3;
            return 0;
        }        
    }
}