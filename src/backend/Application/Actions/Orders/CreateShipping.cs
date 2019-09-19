using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    public class CreateShipping : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public CreateShipping(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order entity)
        {
            var shipping = new Shipping
            {
                Status = "from create"
            };
            db.Shippings.Add(shipping);
            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"create shipping{shipping.Id}"
            };
        }

        public bool IsAvailable(Role role, Order entity)
        {
            return true;
        }
    }
}