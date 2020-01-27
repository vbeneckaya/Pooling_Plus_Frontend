using Application.Shared;
using Domain.Services.DeliveryTypes;
using Domain.Services.UserProvider;

namespace Application.Services.DeliveryTypes
{
    public class DeliveryTypesService : EnumServiceBase<Domain.Enums.DeliveryType>, IDeliveryTypesService
    {
        protected DeliveryTypesService(IUserProvider userIdProvider) : base(userIdProvider)
        {
        }
    }
}