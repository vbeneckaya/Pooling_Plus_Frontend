using Domain.Enums;
using Domain.Persistables;

namespace Domain
{
    public abstract class AppAction<T> : IAppAction<Order>
    {
        public AppColor Color { get; set; }

        public AppAction(AppColor Color)
        {
            this.Color = Color;
        }
        
        public virtual bool Run(User user, Order entity)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool IsAvalible(Role role, Order entity)
        {
            throw new System.NotImplementedException();
        }
    }
}