using System.Collections.Generic;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Orders
{
    public class UnionOrders : IGroupAppAction<Order>
    {
        private readonly AppDbContext db;

        public UnionOrders(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Orange;
        }
        
        public AppColor Color { get; set; }
        public AppActionResult Run(User user, IEnumerable<Order> target)
        {
            var shipping = new Shipping
            {
                Status = "from union"
            };
            db.Shippings.Add(shipping);
            db.SaveChanges();
            return new AppActionResult
            {
                IsError = false,
                Message = $"create shipping{shipping.Id}"
            };
        }

        public bool IsAvailable(Role role, IEnumerable<Order> target)
        {
            return true;
        }
    }
}