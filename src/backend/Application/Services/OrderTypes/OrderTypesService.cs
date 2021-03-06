using Application.Shared;
using Domain.Services.OrderTypes;
using Domain.Services.UserProvider;

namespace Application.Services.DeliveryTypes
{
    public class OrderTypesService : EnumServiceBase<Domain.Enums.OrderType>, IOrderTypesService
    {
        public OrderTypesService(IUserProvider userIdProvider) : base(userIdProvider)
        {
        }
    }
}