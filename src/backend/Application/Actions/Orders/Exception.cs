using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    public class Exception : IAppAction<Order>
    {
        public Exception(AppDbContext _)
        {
            Color = AppColor.Red;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            throw new System.Exception("Это тестовый Exception, см. логи");
        }

        public bool IsAvailable(Role role, Order order)
        {
            return true;
        }
    }
}
