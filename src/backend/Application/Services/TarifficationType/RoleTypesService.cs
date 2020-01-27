using Application.Shared;
using Domain.Services.TarifficationTypes;
using Domain.Services.UserProvider;

namespace Application.Services.RoleTypes
{
    public class TarifficationTypesService : EnumServiceBase<Domain.Enums.TarifficationType>, ITarifficationTypesService
    {
        protected TarifficationTypesService(IUserProvider userIdProvider) : base(userIdProvider)
        {
        }
    }
}