using Application.Shared;
using Domain.Services.DeliveryTypes;
using Domain.Services.UserProvider;

namespace Application.Services.DeliveryTypes
{
    public class DeliveryTypesService : EnumServiceBase<Domain.Enums.DeliveryType>, IDeliveryTypesService
    {
        public DeliveryTypesService(IUserProvider userIdProvider) : base(userIdProvider)
        {
        }
    }
}