using Application.BusinessModels.Orders.Handlers;
using Application.BusinessModels.Shared.BulkUpdates;
using Application.Shared;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.History;
using Domain.Services.UserProvider;
using System;
using System.Globalization;

namespace Application.BusinessModels.Orders.BulkUpdates
{
    public class DeliveryDateBulkUpdate : IBulkUpdate<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;

        public DeliveryDateBulkUpdate(ICommonDataService dataService, IHistoryService historyService)
        {
            _dataService = dataService;
            _historyService = historyService;
        }

        public string FieldName => nameof(Order.DeliveryDate);
        public FiledType FieldType => FiledType.Date;

        public AppActionResult Update(CurrentUserDto user, Order order, string value)
        {
            var setter = new FieldSetter<Order>(order, _historyService);
            setter.UpdateField(x => x.DeliveryDate, ParseDateTime(value), new DeliveryDateHandler(_dataService, _historyService));

            string errors = setter.ValidationErrors;
            bool hasErrors = !string.IsNullOrEmpty(errors);
            if (!hasErrors)
            {
                setter.SaveHistoryLog();
                setter.ApplyAfterActions();
            }

            return new AppActionResult
            {
                IsError = hasErrors,
                Message = errors
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            return (order.Status == OrderState.Created 
                    || order.Status == OrderState.Draft 
                    || (order.Status == OrderState.InShipping && order.OrderShippingStatus == ShippingState.ShippingCreated)) 
                && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }

        private DateTime? ParseDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            if (DateTime.TryParseExact(
                    value,
                    new[] {
                        "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm", "dd.MM.yyyy",
                        "MM/dd/yyyy HH:mm:ss", "MM/dd/yyyy HH:mm", "MM/dd/yyyy",
                        "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd"
                    },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime exactResult))
            {
                return exactResult;
            }
            if (DateTime.TryParse(value, out DateTime result))
            {
                return result;
            }
            return null;
        }
    }
}
