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

            var canEditTariffs = _identityService.HasPermissions(RolePermissions.TariffsEdit);
            var canViewTariffs = _identityService.HasPermissions(RolePermissions.TariffsView);

            if (canViewTariffs)
            {
                var columns = ExtractColumnsFromDto<TariffDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<TariffsService>(),
                    CanCreateByForm = canEditTariffs,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditTariffs,
                    ShowOnHeader = true,
                    Columns = columns
                });
            }

            var canEditWarehouses = _identityService.HasPermissions(RolePermissions.WarehousesEdit);

            if (canEditWarehouses)
            {
                var columns = ExtractColumnsFromDto<WarehouseDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<WarehousesService>(),
                    CanCreateByForm = canEditWarehouses,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditWarehouses,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }

            var canEditArticles = _identityService.HasPermissions(RolePermissions.ArticlesEdit);

            if (canEditArticles)
            {
                var columns = ExtractColumnsFromDto<ArticleDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<ArticlesService>(),
                    CanCreateByForm = canEditArticles,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditArticles,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }

            var canEditPickingTypes = _identityService.HasPermissions(RolePermissions.PickingTypesEdit);

            if (canEditPickingTypes)
            {
                var columns = ExtractColumnsFromDto<PickingTypeDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<PickingTypesService>(),
                    CanCreateByForm = canEditPickingTypes,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditPickingTypes,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }

            var canEditTransportCompanies = _identityService.HasPermissions(RolePermissions.TransportCompaniesEdit);

            if (canEditTransportCompanies)
            {
                var columns = ExtractColumnsFromDto<TransportCompanyDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<TransportCompaniesService>(),
                    CanCreateByForm = canEditTransportCompanies,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditTransportCompanies,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }

            var canEditVehicleTypes = _identityService.HasPermissions(RolePermissions.VehicleTypesEdit);

            if (canEditVehicleTypes)
            {
                var columns = ExtractColumnsFromDto<VehicleTypeDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<VehicleTypesService>(),
                    CanCreateByForm = canEditVehicleTypes,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditVehicleTypes,
                    ShowOnHeader = false,
                    Columns = columns
                });
            }

            var canEditDocumentTypes = _identityService.HasPermissions(RolePermissions.DocumentTypesEdit);

            if (canEditDocumentTypes)
            {
                var columns = ExtractColumnsFromDto<DocumentTypeDto>(roleId);
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<DocumentTypesService>(),
                    CanCreateByForm = canEditDocumentTypes,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditVehicleTypes,
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