using System.Collections.Generic;
using Domain;
using Domain.Enums;
using Domain.Persistables;

namespace Application.Actions.Orders
{
    public class UnionOrders : IGroupAppAction<Order>
    {
        public UnionOrders()
        {
            Color = AppColor.Orange;
        }
        
        public AppColor Color { get; set; }
        public bool Run(User user, IEnumerable<Order> target)
        {
            return true;
        }

        public bool IsAvalible(Role role, IEnumerable<Order> target)
        {
            return true;
        }
    }
}