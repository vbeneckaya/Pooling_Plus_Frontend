using Application.BusinessModels.Orders.Handlers;
using Application.BusinessModels.Shared.BulkUpdates;
using Application.Shared;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.History;
using Domain.Services.UserProvider;
using System;
using System.Globalization;

namespace Application.BusinessModels.Orders.BulkUpdates
{
    public class ShippingDateBulkUpdate : IBulkUpdate<Order>
    {
        private readonly ICommonDataService _dataService;
        private readonly IHistoryService _historyService;
        private readonly IFieldPropertiesService _fieldPropertiesService;

        public ShippingDateBulkUpdate(ICommonDataService dataService, IHistoryService historyService, IFieldPropertiesService fieldPropertiesService)
        {
            _dataService = dataService;
            _historyService = historyService;
            _fieldPropertiesService = fieldPropertiesService;
        }

        public string FieldName => nameof(Order.ShippingDate);
        public FieldType FieldType => FieldType.Date;

        public AppActionResult Update(CurrentUserDto user, Order order, string fieldName, string value)
        {
            var setter = new FieldSetter<Order>(order, _historyService);
            setter.UpdateField(x => x.ShippingDate, ParseDateTime(value), new ShippingDateHandler(_dataService, _historyService));

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
            string fieldName = nameof(order.ShippingDate).ToLowerFirstLetter();
            var fieldAccess = _fieldPropertiesService.GetFieldAccess(FieldPropertiesForEntityType.Orders,
                                                                    (int)order.Status, fieldName,
                                                                    null, role?.Id, null);
            return fieldAccess == FieldPropertiesAccessType.Edit;
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
