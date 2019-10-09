using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared.FormFilters
{
    /// <summary>
    /// Filter for Orders
    /// </summary>
    public class OrderFilterDto: SearchFilterDto
    {
        /// <summary>
        /// Order Number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Order Date
        /// </summary>
        public string OrderDate { get; set; }

        /// <summary>
        /// Order Type
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// SoldTo
        /// </summary>
        public string SoldTo { get; set; }

        /// <summary>
        /// Payer
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// PickingTypeIds List
        /// </summary>
        public string PickingTypeId { get; set; }

        /// <summary>
        /// Min temperature
        /// </summary>
        public string TemperatureMin { get; set; }


        /// <summary>
        /// Max temperature
        /// </summary>
        public string TemperatureMax { get; set; }

        /// <summary>
        /// Shipping Date
        /// </summary>
        public string ShippingDate { get; set; }

        /// <summary>
        /// Shipping Address
        /// </summary>
        public string ShippingAddress { get; set; }

        /// <summary>
        /// Transit Days
        /// </summary>
        public string TransitDays { get; set; }

        /// <summary>
        /// Delivery Region
        /// </summary>
        public string DeliveryRegion { get; set; }

        /// <summary>
        /// Delivery City
        /// </summary>
        public string DeliveryCity { get; set; }

        /// <summary>
        /// Delivery Address
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Delivery Date
        /// </summary>
        public string DeliveryDate { get; set; }
    }
}
