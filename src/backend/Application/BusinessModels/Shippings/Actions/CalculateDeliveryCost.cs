using Application.BusinessModels.Shared.Actions;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.UserProvider;
using System;

namespace Application.BusinessModels.Shippings.Actions
{
    public class CalculateDeliveryCost : IAppAction<Shipping>
    {
        public CalculateDeliveryCost()
        {
            Color = AppColor.Green;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(CurrentUserDto user, Shipping target)
        {
            throw new NotImplementedException();
        }

        public bool IsAvailable(Shipping target)
        {
            return target.Status == ShippingState.ShippingCreated;
        }
    }
}
