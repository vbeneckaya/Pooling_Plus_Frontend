using System;

namespace Domain.Extensions
{
    public class IsDefaultAttribute : Attribute
    {
        public int OrderNumber { get; set; }

        public IsDefaultAttribute(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}
