using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared.FormFilters
{
    public class OrderFilter: SearchFilter
    {
        public string OrderNumber { get; set; }

        public string OrderDate { get; set; }

        public string OrderType { get; set; }

        public string SoldTo { get; set; }

        public string Payer { get; set; }

        public string PickingTypeId { get; set; }

        public string TemperatureMin { get; set; }

        public string TemperatureMax { get; set; }

        public string ShippingDate { get; set; }

        public string ShippingAddress { get; set; }

        public string TransitDays { get; set; }

        public string DeliveryRegion { get; set; }

        public string DeliveryCity { get; set; }

        public string DeliveryAddress { get; set; }

        public string DeliveryDate { get; set; }
    }
}
