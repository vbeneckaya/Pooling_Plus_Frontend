using System;
using System.Collections.Generic;

namespace Domain.Services.Shippings
{
    public class RoutePointDto
    {
        public string WarehouseName { get; set; }
        public string PlannedDate { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureTime { get; set; }
        public string VehicleStatus { get; set; }
        public string Address { get; set; }
        public decimal? TrucksDowntime { get; set; }
        public bool IsLoading { get; set; }
        public List<string> OrderIds { get; set; }
    }
}
