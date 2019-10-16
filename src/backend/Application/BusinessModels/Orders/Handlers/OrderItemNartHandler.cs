using Application.BusinessModels.Shared.Handlers;
using DAL;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using System.Linq;

namespace Application.BusinessModels.Orders.Handlers
{
    public class OrderItemNartHandler : IFieldHandler<OrderItem, string>
    {
        public void AfterChange(OrderItem entity, string oldValue, string newValue)
        {
            if (!_isNew)
            {
                _historyService.Save(entity.OrderId, "orderItemChangeNart", oldValue, newValue);
            }

            var product = _dataService.GetDbSet<Article>().Where(x => x.Nart == newValue).FirstOrDefault();
            if (product != null)
            {
                entity.Spgr = product.Spgr;
                entity.Description = product.Description;
                entity.CountryOfOrigin = product.CountryOfOrigin;
                entity.Ean = product.Ean;
                entity.ShelfLife = product.ShelfLife;
            }
        }

        public string ValidateChange(OrderItem entity, string oldValue, string newValue)
        {
            return null;
        }

        public OrderItemNartHandler(ICommonDataService dataService, IHistoryService historyService, bool isNew)
        {
            _dataService = dataService;
            _historyService = historyService;
            _isNew = isNew;
        }

        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly bool _isNew;
    }
}
