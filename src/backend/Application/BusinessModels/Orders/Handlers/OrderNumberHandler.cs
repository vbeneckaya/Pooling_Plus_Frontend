using Application.BusinessModels.Shared.Handlers;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class OrderNumberHandler : IFieldHandler<Order, string>
    {
        public void AfterChange(Order order, string oldValue, string newValue)
        {
        }

        public string ValidateChange(Order order, string oldValue, string newValue)
        {
            string lang = _userProvider.GetCurrentUser()?.Language;
            var hasSame = _dataService.GetDbSet<Order>().Where(x => x.OrderNumber == newValue && x.Id != order.Id).Any();
            return hasSame ? "duplicateOrderNumber".Translate(lang) : null;
        }

        public OrderNumberHandler(IUserProvider userProvider, ICommonDataService dataService)
        {
            _userProvider = userProvider;
            _dataService = dataService;
        }

        private readonly IUserProvider _userProvider;
        private readonly ICommonDataService _dataService;
    }
}
