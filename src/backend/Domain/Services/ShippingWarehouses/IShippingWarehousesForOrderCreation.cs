﻿using System.Collections.Generic;

namespace Domain.Services.ShippingWarehouses
{
    public interface IShippingWarehousesForOrderCreation
    {
        IEnumerable<ShippingWarehouseDtoForSelect> ForSelect();
    }
}
