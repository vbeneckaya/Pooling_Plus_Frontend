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
    public class ProblemShipping : IAppAction<Shipping>
    {
        private readonly ICommonDataService _dataService;

        private readonly IHistoryService _historyService;

        public AppColor Color { get; set; }
        public string Description { get; set; }

        public ProblemShipping(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
            Color = AppColor.Red;
            Description = "Зафиксировать \"Срыв поставки\"";
        }

        public AppActionResult Run(CurrentUserDto user, Shipping shipping)
        {
            shipping.Status = ShippingState.ShippingProblem;

            foreach (var order in _dataService.GetDbSet<Order>().Where(o => o.ShippingId == shipping.Id))
            {
                order.OrderShippingStatus = shipping.Status;
            }

            _historyService.Save(shipping.Id, "shippingSetProblem", shipping.ShippingNumber);

            return new AppActionResult
            {
                IsError = false,
                Message = "shippingSetProblem".Translate(user.Language, shipping.ShippingNumber)
            };
        }

        public bool IsAvailable(Shipping shipping)
        {
            return IsAvailable(shipping.Status) && shipping.PoolingState == null;
        }

        public bool IsAvailable(ShippingState? shippingStatus)
        {
            return shippingStatus == ShippingState.ShippingConfirmed;
        }
    }
}