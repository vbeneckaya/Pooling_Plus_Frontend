using Application.BusinessModels.Shared.Actions;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Linq;

namespace Application.BusinessModels.Shippings.Actions
{
    /// <summary>
    /// Отменить перевозку
    /// </summary>
    public class DeleteShipping : IAppAction<Shipping>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }
        public string Description { get; set; }

        public DeleteShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Red;
            Description = "Удалить перевозку и все связанные с ней накладные";
        }

        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            var orders = _dataService.GetDbSet<Order>()
                .Where(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id).ToList();
            var ordersIds = _dataService.GetDbSet<Order>()
                .Where(x => x.ShippingId.HasValue && x.ShippingId.Value == shipping.Id)
                .Select(_ => _.Id).ToArray();
            var itemsDbSet = _dataService.GetDbSet<OrderItem>();

            itemsDbSet.RemoveRange(itemsDbSet.Where(x => ordersIds.Contains(x.OrderId)));

            var historyDbSet = _dataService.GetDbSet<HistoryEntry>();
            historyDbSet.RemoveRange(historyDbSet.Where(x => ordersIds.Contains(x.PersistableId)));

            _dataService.GetDbSet<Order>().RemoveRange(orders);

            _dataService.GetDbSet<Shipping>().Remove(shipping);
            historyDbSet.RemoveRange(historyDbSet.Where(x => x.PersistableId == shipping.Id));

            _historyService.Save(shipping.Id, "deleteShipping", shipping.ShippingNumber);

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingDeleted".Translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Shipping shipping)
        {
            return IsAvailable(shipping.Status);
        }

        public bool IsAvailable(ShippingState? shippingStatus)
        {
            return shippingStatus == ShippingState.ShippingCreated
                   || shippingStatus == ShippingState.ShippingRequestSent
                   || shippingStatus == ShippingState.ShippingConfirmed;
        }
    }
}