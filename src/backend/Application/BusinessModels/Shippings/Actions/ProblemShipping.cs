using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Shippings.Actions
{
    public class ProblemShipping : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public ProblemShipping(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Red;
        }

        public AppActionResult Run(User user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingProblem;

            _historyService.Save(shipping.Id, "shippingSetProblem", shipping.ShippingNumber);

            db.SaveChanges();

            return new AppActionResult
            {
                IsError = false,
                Message = $"Срыв поставки по перевозке {shipping.ShippingNumber}"
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingConfirmed) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}