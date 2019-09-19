using Domain;
using Domain.Enums;
using Domain.Persistables;

namespace Application.Actions.Orders
{
    public class CreateShipping : IAppAction<Order>
    {
        public CreateShipping()
        {
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public bool Run(User user, Order entity)
        {
            return true;
        }

        public bool IsAvalible(Role role, Order entity)
        {
            return true;
        }
    }
}