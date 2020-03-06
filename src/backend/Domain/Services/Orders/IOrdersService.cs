using Domain.Persistables;
using Domain.Shared;
using Domain.Shared.FormFilters;
using System.Collections.Generic;
using Domain.Services.Shippings;

namespace Domain.Services.Orders
{
    public interface IOrdersService : IGridService<Order, OrderDto, OrderFormDto, OrderSummaryDto, OrderFilterDto>
    {
        OrderFormDto GetFormByNumber(string orderNumber);
        
        IEnumerable<LookUpDto> FindByNumberAndProvider(NumberSearchFormDto dto);

        OrderFormDto MapFromShippingOrderDtoToFormDto(ShippingOrderDto shippingOrderDto);

        void MapFromDtoToEntity(Order entity, OrderDto dto);
        
        OrderFormDto MapFromDtoToFormDto(OrderDto dto);

        void InitializeNewOrder(Order order, bool isInjection = false);
    }
}