using Domain.Enums;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class AuthorizeByDefaultConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (ShouldApplyConvention(action))
            {
                var attr = (GridPermissionsAttribute)action.Controller.Attributes.First(x => x.GetType() == typeof(GridPermissionsAttribute));

                var actionPermissions = GetActionPermissions(attr);

                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser();

                if (actionPermissions.ContainsKey(action.ActionName))
                {
                    policy = policy.RequireClaim("Permission", actionPermissions[action.ActionName].GetPermissionName());
                    action.Filters.Add(new AuthorizeFilter(policy.Build()));
                }
            }
        }

        private bool ShouldApplyConvention(ActionModel action)
        {
            return action.Controller.Attributes.Any(x => x.GetType() == typeof(GridPermissionsAttribute));
        }

        private Dictionary<string, RolePermissions> GetActionPermissions(GridPermissionsAttribute gridPermissions)
        {
            var dictionary = new Dictionary<string, RolePermissions>();

            if (gridPermissions.SaveOrCreate != RolePermissions.None)
            {
                dictionary.Add("SaveOrCreate", gridPermissions.SaveOrCreate);
            }

            if (gridPermissions.Search != RolePermissions.None)
            {
                dictionary.Add("Search", gridPermissions.Search);
            }

            return dictionary;
        }
    }
}
