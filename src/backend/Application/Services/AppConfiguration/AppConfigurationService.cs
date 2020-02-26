using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application.Services.BodyTypes;
using Application.Services.Clients;
using Application.Services.DocumentTypes;
using Application.Services.Orders;
using Application.Services.PickingTypes;
using Application.Services.ProductTypes;
using Application.Services.Shippings;
using Application.Services.ShippingWarehouses;
using Application.Services.Tariffs;
using Application.Services.Tonnages;
using Application.Services.TransportCompanies;
using Application.Services.VehicleTypes;
using Application.Services.Warehouses;
using Application.Services.Providers;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.AppConfiguration;
using Domain.Services.BodyTypes;
using Domain.Services.Clients;
using Domain.Services.DocumentTypes;
using Domain.Services.FieldProperties;
using Domain.Services.Identity;
using Domain.Services.Orders;
using Domain.Services.PickingTypes;
using Domain.Services.ProductTypes;
using Domain.Services.Providers;
using Domain.Services.Shippings;
using Domain.Services.ShippingWarehouses;
using Domain.Services.Tariffs;
using Domain.Services.Tonnages;
using Domain.Services.TransportCompanies;
using Domain.Services.UserProvider;
using Domain.Services.VehicleTypes;
using Domain.Services.Warehouses;

namespace Application.Services.AppConfiguration
{
    public class AppConfigurationService : AppConfigurationServiceBase, IAppConfigurationService
    {
        private readonly IIdentityService _identityService;
        private readonly IUserProvider _userProvider;
        private readonly ICommonDataService _dataService;
        private readonly IFieldDispatcherService _fieldDispatcherService;
        private readonly IFieldPropertiesService _fieldPropertiesService;

        private readonly Dictionary<Type, Func<Guid?, UserConfigurationDictionaryItem>> _dictionaryConfigurations =
            new Dictionary<Type, Func<Guid?, UserConfigurationDictionaryItem>>();

        public AppConfigurationService(
            IIdentityService identityService,
            IUserProvider userProvider,
            ICommonDataService dataService,
            IFieldDispatcherService fieldDispatcherService,
            IFieldPropertiesService fieldPropertiesService)
        {
            _identityService = identityService;
            _userProvider = userProvider;
            _dataService = dataService;
            _fieldDispatcherService = fieldDispatcherService;
            _fieldPropertiesService = fieldPropertiesService;

            InitDictionariesConfiguration();
        }

        public AppConfigurationDto GetConfiguration()
        {
            var currentUser = _userProvider.GetCurrentUser();
            var roleId = currentUser?.RoleId;
            var res = new AppConfigurationDto
            {
                EditUsers = _identityService.HasPermissions(RolePermissions.UsersEdit),
                EditRoles = _identityService.HasPermissions(RolePermissions.RolesEdit),
                EditFieldProperties = _identityService.HasPermissions(RolePermissions.FieldsSettings),
                ViewReport = _identityService.HasPermissions(RolePermissions.Report),
                Grids = GetGridsConfiguration(roleId),
                Dictionaries = GetDictionariesConfiguration(roleId)
            };
            return res;
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
                    CanImportFromExcel = true,
                    Columns = columns
                });
            }

            if (_identityService.HasPermissions(RolePermissions.ShippingsView))
            {
                var columns = ExtractColumnsFromDto<ShippingDto>(roleId);
                grids.Add(new UserConfigurationGridItem
                {
                    Name = GetName<ShippingsService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.ShippingsCreate),
                    CanViewAdditionSummary = true,
                    CanExportToExcel = true,
                    CanImportFromExcel = false,
                    Columns = columns
                });
            }

            return grids;
        }


        public void InitDictionariesConfiguration()
        {
            var userId = _userProvider.GetCurrentUserId();
            var user = userId == null ? null : _dataService.GetById<User>(userId.Value);

            _dictionaryConfigurations.Add(typeof(WarehouseDto), (roleId) =>
            {
                var canEditWarehouses = _identityService.HasPermissions(RolePermissions.WarehousesEdit);

                if (!canEditWarehouses) return null;

                var columns = ExtractColumnsFromDto<WarehouseDto>(roleId);

//                if (_identityService.HasPermissions(RolePermissions.UsersEdit))
//                    columns = columns.Where(x => x.Name != "CompanyId");

                return new UserConfigurationDictionaryItem
                {
                    Name = GetName<WarehousesService>(),
                    CanCreateByForm = canEditWarehouses,
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = columns
                };
            });

            if (user == null || user != null && !user.ProviderId.HasValue)
            {
                _dictionaryConfigurations.Add(typeof(TariffDto), (roleId) =>
                {
                    var canEditTariffs = _identityService.HasPermissions(RolePermissions.TariffsEdit);
                    var canViewTariffs = _identityService.HasPermissions(RolePermissions.TariffsView);

                    if (!canViewTariffs && !canEditTariffs) return null;

                    var columns = ExtractColumnsFromDto<TariffDto>(roleId);

                    return new UserConfigurationDictionaryItem
                    {
                        Name = GetName<TariffsService>(),
                        CanCreateByForm = canEditTariffs,
                        CanExportToExcel = true,
                        CanImportFromExcel = canEditTariffs,
                        CanDelete = true,
                        ShowOnHeader = true,
                        Columns = columns
                    };
                });
                
                _dictionaryConfigurations.Add(typeof(ShippingWarehouseDto), (roleId) =>
                {
                    var canEditShippingWarehouses =
                        _identityService.HasPermissions(RolePermissions.ShippingWarehousesEdit);
                    var canEditWarehouses = _identityService.HasPermissions(RolePermissions.WarehousesEdit);

                    if (!canEditShippingWarehouses) return null;
                    var columns = ExtractColumnsFromDto<ShippingWarehouseDto>(roleId);

                    return new UserConfigurationDictionaryItem
                    {
                        Name = GetName<ShippingWarehousesService>(),
                        CanCreateByForm = canEditWarehouses,
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = false,
                        Columns = columns
                    };
                });
            }
            else
            {
                _dictionaryConfigurations.Add(typeof(TariffDtoForProvider), (roleId) =>
                {
                    var canEditTariffs = _identityService.HasPermissions(RolePermissions.TariffsEdit);
                    var canViewTariffs = _identityService.HasPermissions(RolePermissions.TariffsView);

                    if (!canViewTariffs && !canEditTariffs) return null;

                    var columns = ExtractColumnsFromDto<TariffDtoForProvider>(roleId);

                    return new UserConfigurationDictionaryItem
                    {
                        Name = GetName<TariffsService>(),
                        CanCreateByForm = canEditTariffs,
                        CanExportToExcel = true,
                        CanImportFromExcel = canEditTariffs,
                        CanDelete = true,
                        ShowOnHeader = true,
                        Columns = columns
                    };
                });
                
                _dictionaryConfigurations.Add(typeof(ShippingWarehouseDtoForProvider), (roleId) =>
                {
                    var canEditShippingWarehouses =
                        _identityService.HasPermissions(RolePermissions.ShippingWarehousesEdit);
                    var canEditWarehouses = _identityService.HasPermissions(RolePermissions.WarehousesEdit);

                    if (!canEditShippingWarehouses) return null;
                    var columns = ExtractColumnsFromDto<ShippingWarehouseDtoForProvider>(roleId);

                    return new UserConfigurationDictionaryItem
                    {
                        Name = GetName<ShippingWarehousesService>(),
                        CanCreateByForm = canEditWarehouses,
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = false,
                        Columns = columns
                    };
                });
            }
            //_dictionaryConfigurations.Add(typeof(ArticleDto), (roleId) =>
            //{
            //    var canEditArticles = _identityService.HasPermissions(RolePermissions.ArticlesEdit);

            //    if (!canEditArticles) return null;

            //    var columns = ExtractColumnsFromDto<ArticleDto>(roleId);
            //    return new UserConfigurationDictionaryItem
            //    {
            //        Name = GetName<ArticlesService>(),
            //        CanCreateByForm = canEditArticles,
            //        CanExportToExcel = true,
            //        CanImportFromExcel = canEditArticles,
            //        ShowOnHeader = false,
            //        Columns = columns
            //    };
            //});

            _dictionaryConfigurations.Add(typeof(ProductTypeDto), (roleId) =>
            {
                var canEditProductTypes = _identityService.HasPermissions(RolePermissions.ProductTypesEdit);

                if (!canEditProductTypes) return null;

                var columns = ExtractColumnsFromDto<ProductTypeDto>(roleId);
                return new UserConfigurationDictionaryItem
                {
                    Name = GetName<ProductTypesService>(),
                    CanCreateByForm = canEditProductTypes,
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = columns
                };
            });

            _dictionaryConfigurations.Add(typeof(PickingTypeDto), (roleId) =>
            {
                var canEditPickingTypes = _identityService.HasPermissions(RolePermissions.PickingTypesEdit);

                if (!canEditPickingTypes) return null;

                var columns = ExtractColumnsFromDto<PickingTypeDto>(roleId);
                return new UserConfigurationDictionaryItem
                {
                    Name = GetName<PickingTypesService>(),
                    CanCreateByForm = canEditPickingTypes,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditPickingTypes,
                    ShowOnHeader = false,
                    Columns = columns
                };
            });

            _dictionaryConfigurations.Add(typeof(TransportCompanyDto), (roleId) =>
            {
                var canEditTransportCompanies = _identityService.HasPermissions(RolePermissions.TransportCompaniesEdit);

                if (!canEditTransportCompanies) return null;

                var columns = ExtractColumnsFromDto<TransportCompanyDto>(roleId);
                return new UserConfigurationDictionaryItem
                {
                    Name = GetName<TransportCompaniesService>(),
                    CanCreateByForm = canEditTransportCompanies,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditTransportCompanies,
                    ShowOnHeader = false,
                    Columns = columns
                };
            });

            _dictionaryConfigurations.Add(typeof(ClientDto), (roleId) =>
            {
                var canEditClients = _identityService.HasPermissions(RolePermissions.ClientsEdit);

                if (!canEditClients) return null;

                var columns = ExtractColumnsFromDto<ClientDto>(roleId);
                return new UserConfigurationDictionaryItem
                {
                    Name = GetName<ClientsService>(),
                    CanCreateByForm = canEditClients,
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = columns
                };
            });

            _dictionaryConfigurations.Add(typeof(ProviderDto), (roleId) =>
            {
                var canEditProviders = _identityService.HasPermissions(RolePermissions.ProvidersEdit);

                if (!canEditProviders) return null;

                var columns = ExtractColumnsFromDto<ProviderDto>(roleId);

                var role = _dataService.GetById<Role>(roleId.Value);
                if (role.RoleType != Domain.Enums.RoleTypes.Administrator)
                    columns = columns.Where(x => x.Name != nameof(Provider.ReportId));

                return new UserConfigurationDictionaryItem
                {
                    Name = GetName<ProvidersService>(),
                    CanCreateByForm = canEditProviders,
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = columns
                };
                ;
            });

            _dictionaryConfigurations.Add(typeof(VehicleTypeDto), (roleId) =>
            {
                var canEditVehicleTypes = _identityService.HasPermissions(RolePermissions.VehicleTypesEdit);

                if (!canEditVehicleTypes) return null;

                var columns = ExtractColumnsFromDto<VehicleTypeDto>(roleId);
                return new UserConfigurationDictionaryItem
                {
                    Name = GetName<VehicleTypesService>(),
                    CanCreateByForm = canEditVehicleTypes,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditVehicleTypes,
                    ShowOnHeader = false,
                    Columns = columns
                };
            });

            _dictionaryConfigurations.Add(typeof(BodyTypeDto), (roleId) =>
            {
                var canEditVehicleTypes = _identityService.HasPermissions(RolePermissions.VehicleTypesEdit);

                if (!canEditVehicleTypes) return null;

                var bodyTypeColumns = ExtractColumnsFromDto<BodyTypeDto>(roleId);

                return new UserConfigurationDictionaryItem
                {
                    Name = GetName<BodyTypesService>(),
                    CanCreateByForm = canEditVehicleTypes,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditVehicleTypes,
                    ShowOnHeader = false,
                    Columns = bodyTypeColumns
                };
            });

            _dictionaryConfigurations.Add(typeof(TonnageDto), (roleId) =>
            {
                var canEditVehicleTypes = _identityService.HasPermissions(RolePermissions.VehicleTypesEdit);

                if (!canEditVehicleTypes) return null;

                var tonnageColumns = ExtractColumnsFromDto<TonnageDto>(roleId);

                return new UserConfigurationDictionaryItem
                {
                    Name = GetName<TonnagesService>(),
                    CanCreateByForm = canEditVehicleTypes,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditVehicleTypes,
                    ShowOnHeader = false,
                    Columns = tonnageColumns
                };
            });

            _dictionaryConfigurations.Add(typeof(DocumentTypeDto), (roleId) =>
            {
                var canEditDocumentTypes = _identityService.HasPermissions(RolePermissions.DocumentTypesEdit);
                var canEditVehicleTypes = _identityService.HasPermissions(RolePermissions.VehicleTypesEdit);

                if (!canEditDocumentTypes) return null;

                var columns = ExtractColumnsFromDto<DocumentTypeDto>(roleId);
                return new UserConfigurationDictionaryItem
                {
                    Name = GetName<DocumentTypesService>(),
                    CanCreateByForm = canEditDocumentTypes,
                    CanExportToExcel = true,
                    CanImportFromExcel = canEditVehicleTypes,
                    ShowOnHeader = false,
                    Columns = columns
                };
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private IEnumerable<UserConfigurationDictionaryItem> GetDictionariesConfiguration(Guid? roleId)
        {
            return _dictionaryConfigurations
                .Select(i => i.Value(roleId))
                .Where(i => i != null);
        }

        private FieldPropertiesForEntityType? GetFieldPropertyForEntity<TDto>()
        {
            if (typeof(TDto) == typeof(OrderDto))
            {
                return FieldPropertiesForEntityType.Orders;
            }
            else if (typeof(TDto) == typeof(ShippingDto))
            {
                return FieldPropertiesForEntityType.Shippings;
            }
            else
            {
                return null;
            }
        }

        private IEnumerable<UserConfigurationGridColumn> ExtractColumnsFromDto<TDto>(Guid? roleId)
        {
            var fields = _fieldDispatcherService.GetDtoFields<TDto>();

            var forEntity = GetFieldPropertyForEntity<TDto>();
            if (forEntity.HasValue)
            {
                var availableFieldNames = _fieldPropertiesService.GetAvailableFields(forEntity.Value, roleId, null);
                fields = fields.Where(x => availableFieldNames.Any(y => string.Compare(x.Name, y, true) == 0));
            }

            foreach (var field in fields.OrderBy(f => f.OrderNumber))
            {
                if (string.IsNullOrEmpty(field.ReferenceSource))
                {
                    yield return new UserConfigurationGridColumn(field);
                }
                else
                {
                    yield return new UserConfigurationGridColumnWhitchSource(field);
                }
            }
        }
    }
}