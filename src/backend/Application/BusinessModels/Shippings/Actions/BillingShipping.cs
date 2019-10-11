using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;

namespace Application.BusinessModels.Shippings.Actions
{
    public class BillingShipping : IAppAction<Shipping>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public BillingShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            this._dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Blue;
        }

        public AppActionResult Run(User user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingBillSend;

            _historyService.Save(shipping.Id, "shippingSetBillSend", shipping.ShippingNumber);

            _dataService.SaveChanges();

            return new AppActionResult
            {
                IsError = false,
                Message = $"Выставлен счет по перевозке {shipping.ShippingNumber}"
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingCompleted) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}