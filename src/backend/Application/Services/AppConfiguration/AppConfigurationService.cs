using Application.Services.Articles;
using Application.Services.DocumentTypes;
using Application.Services.Orders;
using Application.Services.PickingTypes;
using Application.Services.Shippings;
using Application.Services.Tariffs;
using Application.Services.TransportCompanies;
using Application.Services.VehicleTypes;
using Application.Services.Warehouses;
using Domain.Enums;
using Domain.Services.AppConfiguration;
using Domain.Services.Articles;
using Domain.Services.DocumentTypes;
using Domain.Services.FieldProperties;
using Domain.Services.Identity;
using Domain.Services.Orders;
using Domain.Services.PickingTypes;
using Domain.Services.Shippings;
using Domain.Services.Tariffs;
using Domain.Services.TransportCompanies;
using Domain.Services.UserProvider;
using Domain.Services.VehicleTypes;
using Domain.Services.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.AppConfiguration
{

    public class AppConfigurationService : AppConfigurationServiceBase ,IAppConfigurationService
    {
        private readonly IIdentityService _identityService;
        private readonly IUserProvider _userProvider;
        private readonly IFieldDispatcherService _fieldDispatcherService;

        public AppConfigurationService(
            IIdentityService identityService, 
            IUserProvider userProvider, 
            IFieldDispatcherService fieldDispatcherService)
        {
            _identityService = identityService;
            _userProvider = userProvider;
            _fieldDispatcherService = fieldDispatcherService;
        }
        
        public AppConfigurationDto GetConfiguration()
        {
            var roleId = _userProvider.GetCurrentUser()?.RoleId;
            return new AppConfigurationDto
            {
                EditUsers = _identityService.HasPermissions(RolePermissions.UsersEdit),
                EditRoles = _identityService.HasPermissions(RolePermissions.RolesEdit),
                Grids = GetGridsConfiguration(roleId), 
                Dictionaries = GetDictionariesConfiguration(roleId)                
            };
        }

        public IEnumerable<UserConfigurationGridItem> GetGridsConfiguration(Guid? roleId)
        {
            var grids = new List<UserConfigurationGridItem>();

            if (_identityService.HasPermissions(RolePermissions.OrdersView))
            {
                var columns = ExtractColumnsFromDto<OrderDto>(roleId);
                grids.Add(new UserConfigurationGridItem
                {
                    Name = GetName<OrdersService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.OrdersCreate),
                    CanViewAdditionSummary = true,
                    CanExportToExcel = true,
                    CanImportFromExcel = false,
                    Columns = columns
                });
            }

            if (_identityService.HasPermissions(RolePermissions.ShippingsView))
            {
                var columns = ExtractColumnsFromDto<ShippingDto>(roleId);
                grids.Add(new UserConfigurationGridItem
                {
                    Name = GetName<ShippingsService>(),
                    CanCreateByForm = false,
                    CanViewAdditionSummary = true,
                    CanExportToExcel = true,
                    CanImportFromExcel = false,
                    Columns = columns
                });
            }

            return grids;
        }

        public IEnumerable<UserConfigurationDictionaryItem> GetDictionariesConfiguration(Guid? roleId)
        {
            var dicts = new List<UserConfigurationDictionaryItem>();

            if (_identityService.HasPermissions(RolePermissions.TariffsView))
            {
                var columns = ExtractColumnsFromDto<TariffDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<TariffsService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.TariffsEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = true,
                    Columns = columns
                });
            }

            if (_identityService.HasPermissions(RolePermissions.WarehousesEdit))
            {
                var columns = ExtractColumnsFromDto<WarehouseDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<WarehousesService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.WarehousesEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }

            if (_identityService.HasPermissions(RolePermissions.ArticlesEdit))
            {
                var columns = ExtractColumnsFromDto<ArticleDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<ArticlesService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.ArticlesEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }

            if (_identityService.HasPermissions(RolePermissions.PickingTypesEdit))
            {
                var columns = ExtractColumnsFromDto<PickingTypeDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<PickingTypesService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.PickingTypesEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }

            if (_identityService.HasPermissions(RolePermissions.TransportCompaniesEdit))
            {
                var columns = ExtractColumnsFromDto<TransportCompanyDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<TransportCompaniesService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.TransportCompaniesEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }

            if (_identityService.HasPermissions(RolePermissions.VehicleTypesEdit))
            {
                var columns = ExtractColumnsFromDto<VehicleTypeDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<VehicleTypesService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.VehicleTypesEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }

            if (_identityService.HasPermissions(RolePermissions.DocumentTypesEdit))
            {
                var columns = ExtractColumnsFromDto<DocumentTypeDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<DocumentTypesService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.DocumentTypesEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }
                
            return dicts;
        }

        private IEnumerable<UserConfigurationGridColumn> ExtractColumnsFromDto<TDto>(Guid? roleId)
        {
            var fields = _fieldDispatcherService.GetDtoFields<TDto>();
            foreach (var field in fields.OrderBy(f => f.OrderNumber))
            {
                if (string.IsNullOrEmpty(field.ReferenceSource))
                {
                    yield return new UserConfigurationGridColumn(field.Name, field.FieldType, field.IsDefault);
                }
                else
                {
                    yield return new UserConfigurationGridColumnWhitchSource(field.Name, field.FieldType, field.ReferenceSource, field.IsDefault, field.ShowRawReferenceValue);
                }
            }
        }

    }
}