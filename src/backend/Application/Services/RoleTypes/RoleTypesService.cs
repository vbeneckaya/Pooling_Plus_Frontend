using Application.Shared;
using Domain.Services.Roles;
using Domain.Services.UserProvider;

namespace Application.Services.RoleTypes
{
    public class RoleTypesService : EnumServiceBase<Domain.Enums.RoleTypes>, IRoleTypesService
    {
        public RoleTypesService(IUserProvider userIdProvider) : base(userIdProvider)
        {
        }
    }
}