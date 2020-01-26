using Domain.Persistables;
using Domain.Services.Permissions;
using Domain.Shared;
using System;
using System.Collections.Generic;

namespace Domain.Services.Roles
{
    public interface IRolesService : IDictonaryService<Role, RoleDto>
    {
        ValidateResult SetActive(Guid id, bool active);

        IEnumerable<PermissionInfo> GetAllPermissions();

        RoleActionsDto GetAllActions();
        
        IEnumerable<CompaniesByRole> GetCompanyTypeByRole(Guid id);

        RoleDto MapFromEntityToDto(Role entity);

//        IEnumerable<LookUpDto> ForSelectByCompany(Guid? companyId);
    }
}