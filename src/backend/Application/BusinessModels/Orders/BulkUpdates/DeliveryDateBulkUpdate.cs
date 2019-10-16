using Application.BusinessModels.Shared.BulkUpdates;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Orders.BulkUpdates
{
    public class DeliveryDateBulkUpdate : IBulkUpdate<Order>
    {
        public string FieldName => nameof(Order.DeliveryDate);
        public FiledType FieldType => FiledType.DateTime;

        public AppActionResult Update(CurrentUserDto user, Order order, string value)
        {
            return new AppActionResult();
        }

        public bool IsAvailable(Role role, Order order)
        {
            return (order.Status == OrderState.Created 
                    || order.Status == OrderState.Draft 
                    || (order.Status == OrderState.InShipping && order.OrderShippingStatus == ShippingState.ShippingCreated)) 
                && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }
}
