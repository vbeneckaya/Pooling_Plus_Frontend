using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.Permissions;
using Domain.Services.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Services.Permissions
{
    public class PermissionsService : IPermissionsService
    {
        private readonly ICommonDataService _dataService;

        private readonly ITranslationsService _translationService;

        public PermissionsService(ICommonDataService dataService, ITranslationsService translationService)
        {
            _dataService = dataService;
            _translationService = translationService;
        }

        /// <summary>
        /// Get permissions for role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public IEnumerable<RolePermission> GetPermissionsForRole(string roleName)
        {
            return _dataService.GetDbSet<RolePermission>()
                .Where(i => i.Role.Name == roleName);
        }

        /// <summary>
        /// Get permissions for role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public IEnumerable<PermissionInfo> GetPermissionsInfoForRole(string roleName)
        {
            return this.GetPermissionsForRole(roleName)
                .Select(i => this.GetInfoByCode(i.PermissionCode));
        }

        /// <summary>
        /// Get permissions for role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public IEnumerable<PermissionInfo> GetPermissionsInfoForRole(Guid? roleId)
        {
            return this.GetPermissionsForRole(roleId)
                .Select(i => this.GetInfoByCode(i.PermissionCode));
        }

        /// <summary>
        /// Get permissions for role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public IEnumerable<RolePermission> GetPermissionsForRole(Guid? roleId)
        {
            if (!roleId.HasValue) return null;

            return _dataService.GetDbSet<RolePermission>()
                .Where(i => i.RoleId == roleId);
        }

        /// <summary>
        /// Update permissions
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="permissions"></param>
        public void UpdateRolePermissions(string roleName, IEnumerable<RolePermissions> permissions)
        {
            var role = _dataService.GetDbSet<Role>().FirstOrDefault(i => i.Name == roleName);

            var existingPermissions = this.GetPermissionsForRole(roleName);

            var toRemove = existingPermissions.Where(i => !permissions.Contains(i.PermissionCode)).ToList();
            var toAddCodes = permissions.Where(i => !existingPermissions.Select(j => j.PermissionCode).Contains(i)).ToList();

            _dataService.GetDbSet<RolePermission>().AddRange(toAddCodes.Select(i => new RolePermission
            {
                PermissionCode = i,
                RoleId = role.Id
            }));

            _dataService.GetDbSet<RolePermission>().RemoveRange(toRemove);

            _dataService.SaveChanges();
        }

        private PermissionInfo GetInfoByCode(RolePermissions permission)
        {
            var translation = _translationService.FindByKey(permission.GetPermissionName());

            return new PermissionInfo
            {
                Code = permission,
                Name = permission.ToString(),
                Translations = new TranslationDto
                {
                    Id = translation.Id.ToString(),
                    Name = translation.Name,
                    En = translation.En,
                    Ru = translation.Ru
                }
            };
        }
    }
}
