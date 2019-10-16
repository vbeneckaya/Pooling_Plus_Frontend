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
        [FilterField(Type = FilterFieldType.String)]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Shipping Number
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string ShippingNumber { get; set; }

        /// <summary>
        /// ClientName
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string ClientName { get; set; }

        /// <summary>
        /// Order Date
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange)]
        public string OrderDate { get; set; }

        /// <summary>
        /// Order Type
        /// </summary>
        [FilterField(Type = FilterFieldType.Enum)]
        public string OrderType { get; set; }

        /// <summary>
        /// SoldTo
        /// </summary>
        [FilterField(Type = FilterFieldType.Options)]
        public string SoldTo { get; set; }

        /// <summary>
        /// Payer
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string Payer { get; set; }

        /// <summary>
        /// PickingTypeIds List
        /// </summary>
        [FilterField(Type = FilterFieldType.Options)]
        public string PickingTypeId { get; set; }

        /// <summary>
        /// Min temperature
        /// </summary>
        [FilterField(Type = FilterFieldType.Integer)]
        public string TemperatureMin { get; set; }


        /// <summary>
        /// Max temperature
        /// </summary>
        [FilterField(Type = FilterFieldType.Integer)]
        public string TemperatureMax { get; set; }

        /// <summary>
        /// Shipping Date
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange)]
        public string ShippingDate { get; set; }

        /// <summary>
        /// Shipping Address
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string ShippingAddress { get; set; }

        /// <summary>
        /// Transit Days
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange)]
        public string TransitDays { get; set; }

        /// <summary>
        /// Delivery Region
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string DeliveryRegion { get; set; }

        /// <summary>
        /// Delivery City
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string DeliveryCity { get; set; }

        /// <summary>
        /// Delivery Address
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Delivery Date
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange)]
        public string DeliveryDate { get; set; }

        /// <summary>
        /// Count of articles
        /// </summary>
        [FilterField(Type = FilterFieldType.Integer)]
        public string ArticlesCount { get; set; }

        /// <summary>
        /// Boxes Count
        /// </summary>
        [FilterField(Type = FilterFieldType.Integer)]
        public string BoxesCount { get; set; }

        /// <summary>
        /// Confirmed Boxes Count
        /// </summary>
        [FilterField(Type = FilterFieldType.Integer)]
        public string ConfirmedBoxesCount { get; set; }

        /// <summary>
        /// Pallets Count
        /// </summary>
        [FilterField(Type = FilterFieldType.Integer)]
        public string PalletsCount { get; set; }

        /// <summary>
        /// Actual Pallets Count
        /// </summary>
        [FilterField(Type = FilterFieldType.Integer)]
        public string ActualPalletsCount { get; set; }

        /// <summary>
        /// Weight Kg
        /// </summary>
        [FilterField(Type = FilterFieldType.Decimal)]
        public string WeightKg { get; set; }

        /// <summary>
        /// Actual Weight Kg
        /// </summary>
        [FilterField(Type = FilterFieldType.Decimal)]
        public string ActualWeightKg { get; set; }

        /// <summary>
        /// OrderAmountExcludingVAT
        /// </summary>
        [FilterField(Type = FilterFieldType.Decimal)]
        public string OrderAmountExcludingVAT { get; set; }

        /// <summary>
        /// Bdf Invoice Number
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string BdfInvoiceNumber { get; set; }

        /// <summary>
        /// Loading Arrival Time
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange)]
        public string LoadingArrivalTime { get; set; }

        /// <summary>
        /// Loading DepartureT ime
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange)]
        public string LoadingDepartureTime { get; set; }

        /// <summary>
        /// Unloading Arrival Date
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange, Searched = false)]
        public string UnloadingArrivalDate { get; set; }

        /// <summary>
        /// Unloading Arrival Time
        /// </summary>
        public string UnloadingArrivalTime { get; set; }

        /// <summary>
        /// Unloading Departure Date
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange, Searched = false)]
        public string UnloadingDepartureDate { get; set; }

        /// <summary>
        /// Unloading Departure Time
        /// </summary>
        public string UnloadingDepartureTime { get; set; }

        /// <summary>
        /// Trucks Downtime
        /// </summary>
        [FilterField(Type = FilterFieldType.Decimal)]
        public string TrucksDowntime { get; set; }

        /// <summary>
        /// Return Information
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string ReturnInformation { get; set; }

        /// <summary>
        /// Return Shipping Account No
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string ReturnShippingAccountNo { get; set; }

        /// <summary>
        /// Planned Return Date
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange)]
        public string PlannedReturnDate { get; set; }

        /// <summary>
        /// Actual Return Date
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange)]
        public string ActualReturnDate { get; set; }

        /// <summary>
        /// Major Adoption Number
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string MajorAdoptionNumber { get; set; }

        /// <summary>
        /// Client Avisation Time 
        /// </summary>
        public string ClientAvisationTime { get; set; }

        /// <summary>
        /// Order Comments
        /// </summary>
        [FilterField(Type = FilterFieldType.String)]
        public string OrderComments { get; set; }

        /// <summary>
        /// Order Creation Date
        /// </summary>
        [FilterField(Type = FilterFieldType.DateRange)]
        public string OrderCreationDate { get; set; }

        /// <summary>
        /// Shipping Ids list
        /// </summary>
        [FilterField(Type = FilterFieldType.Options)]
        public string ShippingId { get; set; }
    }
}
