using System;
using System.Diagnostics;
using Domain.Enums;

namespace Domain.Persistables
{
    public class FieldPropertyItem : IPersistable
    {
        public Guid Id { get; set; }
        
        public FieldPropertiesForEntityType ForEntity { get; set; }


        public Guid? CompanyId { get; set; }

        public Guid? RoleId { get; set; }

        public string FieldName { get; set; }
        public string State { get; set; }
        public FieldPropertiesAccessType AccessType { get; set; }
        
        public int CompareTo(object o)
        {
            FieldPropertyItem nextItem = o as FieldPropertyItem;
            if (nextItem != null)
                return GetPoints(nextItem) - GetPoints(this);
            else
                return -1;
        }

        private int GetPoints(FieldPropertyItem item)
        {
            int result = 0;
            if (item.RoleId != null) result += 2;
            if (item.CompanyId != null) result += 1;
            return result;
        }
    }
}