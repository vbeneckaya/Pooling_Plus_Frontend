using System.Collections;
using System.Collections.Generic;

namespace Integrations.Pooling.Dtos
{
    public class PoolingCalendarFilters
    {
        public IEnumerable<PoolingCalendarFilterItem> ProductTypes { get; set; }
        public IEnumerable<PoolingCalendarFilterItem> Regions { get; set; }
        public IEnumerable<PoolingCalendarFilterItem> Carriers { get; set; }
        public IEnumerable<PoolingCalendarFilterItem> Clients { get; set; }
        public IEnumerable<PoolingCalendarFilterItem> CarTypes { get; set; }
        public IEnumerable<PoolingCalendarFilterItem> DistributionCenters { get; set; }
    }

    public class PoolingCalendarFilterItem
    {
        public bool IsAvailable { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}