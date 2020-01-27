using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Extensions
{
    public static class RoleTypeExtension
    {
        public const string ClaimType = "RoleType";

        public static string GetPermissionName(this RoleTypes roleType)
        {
            return $"{ClaimType}.{roleType.ToString()}";
        }
    }
}
