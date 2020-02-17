using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class HasPermissionAttribute: AuthorizeAttribute
    {
        public HasPermissionAttribute(RolePermissions permission) : base(permission.GetPermissionName())
        { 
        }
    }
}
