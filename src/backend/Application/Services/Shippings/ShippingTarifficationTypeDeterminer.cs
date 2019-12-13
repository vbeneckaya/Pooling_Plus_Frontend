using System.Collections.Generic;
using System.Linq;
using Domain.Enums;
using Domain.Persistables;

namespace Application.Services.Shippings
{
    public class ShippingTarifficationTypeDeterminer : IShippingTarifficationTypeDeterminer
    {
        public TarifficationType GetTarifficationTypeForOrders(IEnumerable<Order> orders)
        {
            if (orders.Any(x => 
                    !string.IsNullOrEmpty(x.DeliveryRegion) && 
                    (x.DeliveryRegion.Contains("Москва г") ||
                     x.DeliveryRegion.Contains("Московская обл") ||
                     x.DeliveryRegion.Contains("Новосибирская обл"))
                ) || 
                orders.Sum(x => x.PalletsCount) > 24)
                return TarifficationType.Ftl;
            
            return TarifficationType.Ltl;
        }
    }
}