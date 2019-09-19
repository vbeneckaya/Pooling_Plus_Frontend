using System.Collections.Generic;
using Domain;
using Domain.Enums;
using Domain.Persistables;

namespace Application.Actions.Orders
{
    public class CreateShipping : AppAction<Order>
    {
        public CreateShipping() : base(AppColor.Orange)
        {
        }

        public override bool Run(User user, Order entity)
        {
            return true;
        }

        public override bool IsAvalible(Role role, Order entity)
        {
            return true;
        }
    }
    
    public class UnoinOrder : AppAction<Order>
    {
        public UnoinOrder() : base(AppColor.Orange)
        {
        }
        
        
        public override bool IsAvalible(Role role, Order entity)
        {
            return true;
        }
    }
}