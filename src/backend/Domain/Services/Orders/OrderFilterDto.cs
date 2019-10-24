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
        /// Shipping Number
        /// </summary>
        public string ShippingNumber { get; set; }

        /// <summary>
        /// ClientName
        /// </summary>
        public string ClientName { get; set; }

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

        /// <summary>
        /// Count of articles
        /// </summary>
        public string ArticlesCount { get; set; }

        /// <summary>
        /// Boxes Count
        /// </summary>
        public string BoxesCount { get; set; }

        /// <summary>
        /// Confirmed Boxes Count
        /// </summary>
        public string ConfirmedBoxesCount { get; set; }

        /// <summary>
        /// Pallets Count
        /// </summary>
        public string PalletsCount { get; set; }

        /// <summary>
        /// Actual Pallets Count
        /// </summary>
        public string ActualPalletsCount { get; set; }

        /// <summary>
        /// Weight Kg
        /// </summary>
        public string WeightKg { get; set; }

        /// <summary>
        /// Actual Weight Kg
        /// </summary>
        public string ActualWeightKg { get; set; }

        /// <summary>
        /// OrderAmountExcludingVAT
        /// </summary>
        public string OrderAmountExcludingVAT { get; set; }

        /// <summary>
        /// Bdf Invoice Number
        /// </summary>
        public string BdfInvoiceNumber { get; set; }

        /// <summary>
        /// Loading Arrival Time
        /// </summary>
        public string LoadingArrivalTime { get; set; }

        /// <summary>
        /// Loading DepartureT ime
        /// </summary>
        public string LoadingDepartureTime { get; set; }

        /// <summary>
        /// Unloading Arrival Date
        /// </summary>
        public string UnloadingArrivalDate { get; set; }

        /// <summary>
        /// Unloading Arrival Time
        /// </summary>
        public string UnloadingArrivalTime { get; set; }

        /// <summary>
        /// Unloading Departure Date
        /// </summary>
        public string UnloadingDepartureDate { get; set; }

        /// <summary>
        /// Unloading Departure Time
        /// </summary>
        public string UnloadingDepartureTime { get; set; }

        /// <summary>
        /// Trucks Downtime
        /// </summary>
        public string TrucksDowntime { get; set; }

        /// <summary>
        /// Return Information
        /// </summary>
        public string ReturnInformation { get; set; }

        /// <summary>
        /// Return Shipping Account No
        /// </summary>
        public string ReturnShippingAccountNo { get; set; }

        /// <summary>
        /// Planned Return Date
        /// </summary>
        public string PlannedReturnDate { get; set; }

        /// <summary>
        /// Actual Return Date
        /// </summary>
        public string ActualReturnDate { get; set; }

        /// <summary>
        /// Major Adoption Number
        /// </summary>
        public string MajorAdoptionNumber { get; set; }

        /// <summary>
        /// Client Avisation Time 
        /// </summary>
        public string ClientAvisationTime { get; set; }

        /// <summary>
        /// Order Comments
        /// </summary>
        public string OrderComments { get; set; }

        /// <summary>
        /// Order Creation Date
        /// </summary>
        public string OrderCreationDate { get; set; }

        /// <summary>
        /// Shipping Ids list
        /// </summary>
        public string ShippingId { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Order Shipping Status
        /// </summary>
        public string OrderShippingStatus { get; set; }

        /// <summary>
        /// Delivery Status
        /// </summary>
        public string DeliveryStatus { get; set; }

        /// <summary>
        /// Shipping Status
        /// </summary>
        public string ShippingStatus { get; set; }

    }
}
