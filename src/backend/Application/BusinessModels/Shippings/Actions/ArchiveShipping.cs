using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Shippings.Actions
{
    public class ArchiveShipping : IAppAction<Shipping>
    {
        private readonly AppDbContext db;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public ArchiveShipping(AppDbContext db, IHistoryService historyService)
        {
            this.db = db;
            _historyService = historyService;
            Color = AppColor.Teal;
        }

        public AppActionResult Run(User user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingArhive;

            _historyService.Save(shipping.Id, "shippingSetArchived", shipping.ShippingNumber);

            db.SaveChanges();

            return new AppActionResult
            {
                IsError = false,
                Message = $"Перевозка {shipping.ShippingNumber} перенесена в архив"
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingBillSend) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}