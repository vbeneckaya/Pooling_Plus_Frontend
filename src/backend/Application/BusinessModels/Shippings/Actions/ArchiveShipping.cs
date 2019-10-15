using DAL;
using DAL.Services;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Linq;

namespace Application.BusinessModels.Shippings.Actions
{
    public class ArchiveShipping : IAppAction<Shipping>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }

        public ArchiveShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            this._dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Teal;
        }

        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingArhive;

            foreach (var order in _dataService.GetDbSet<Order>().Where(o => o.ShippingId == shipping.Id))
            {
                order.OrderShippingStatus = shipping.Status;
            }

            _historyService.Save(shipping.Id, "shippingSetArchived", shipping.ShippingNumber);

            _dataService.SaveChanges();

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetArchived".translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Role role, Shipping shipping)
        {
            return (shipping.Status == ShippingState.ShippingBillSend) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}