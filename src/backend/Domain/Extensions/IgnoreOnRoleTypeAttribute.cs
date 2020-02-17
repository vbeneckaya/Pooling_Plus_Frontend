using Domain.Enums;
using System;

namespace Domain.Extensions
{
    public class IgnoreOnRoleTypeAttribute : Attribute
    {
        public RoleTypes RoleType { get; set; }

        public IgnoreOnRoleTypeAttribute(RoleTypes type)
        {
            RoleType = type;
        }
    }
}
