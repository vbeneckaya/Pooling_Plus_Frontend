using System.Collections.Generic;
using Application.Services.Translations;
/*start of using application service*/
using Application.Services.Orders;
using Application.Services.Shippings;
using Application.Services.Tariffs;
using Application.Services.Warehouses;
using Application.Services.Articles;
using Application.Services.TransportCompanies;
/*end of using application service*/
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.AppConfiguration;
/*start of using domain service*/
using Domain.Services.Orders;
using Domain.Services.Shippings;
using Domain.Services.Tariffs;
using Domain.Services.Warehouses;
using Domain.Services.Articles;
using Domain.Services.TransportCompanies;
using Application.Services.DocumentTypes;
using Domain.Services.DocumentTypes;
using Application.Services.PickingTypes;
using Application.Services.VehicleTypes;
using Domain.Services.VehicleTypes;
using Domain.Services.PickingTypes;
using Domain.Services.Identity;
using System.Linq;
using Domain.Extensions;
using System;
/*end of using domain service*/

namespace Application.Services.AppConfiguration
{
    
    public class AppConfigurationService : AppConfigurationServiceBase ,IAppConfigurationService 
    {

        private readonly IIdentityService _identityService;

        public AppConfigurationService(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        
        public AppConfigurationDto GetConfiguration()
        {
            return new AppConfigurationDto
            {
                EditUsers = _identityService.HasPermissions(RolePermissions.UsersEdit),
                EditRoles = _identityService.HasPermissions(RolePermissions.RolesEdit),
                Grids = GetGridsConfiguration(), 
                Dictionaries = GetDictionariesConfiguration()                
            };
        }

        public IEnumerable<UserConfigurationGridItem> GetGridsConfiguration()
        {
            var grids = new List<UserConfigurationGridItem>();

            var user = _identityService.GetUserInfo();

            // Orders

            if (_identityService.HasPermissions(RolePermissions.OrdersView))
            {
                var columns = ExtractColumnsFromDto<OrderDto>();
                grids.Add(
                    new UserConfigurationGridItem
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
                var columns = ExtractColumnsFromDto<ShippingDto>();
                grids.Add(new UserConfigurationGridItem
                    {
                        Name = GetName<ShippingsService>(),
                        CanCreateByForm = _identityService.HasPermissions(RolePermissions.ShippingsView),
                        CanViewAdditionSummary = true,
                        CanExportToExcel = true,
                        CanImportFromExcel = false,
                        Columns = columns
                });
            }

            return grids;
        }

        public IEnumerable<UserConfigurationDictionaryItem> GetDictionariesConfiguration()
        {
            var dicts = new List<UserConfigurationDictionaryItem>();

            if (_identityService.HasPermissions(RolePermissions.TariffsView))
            {
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<TariffsService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.TariffsEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = true,
                    Columns = new List<UserConfigurationGridColumn>
                    {
                        /*start of add field for Tariffs*/
                        new UserConfigurationGridColumn(nameof(TariffDto.ShipmentCity), FieldType.Text),
                        new UserConfigurationGridColumn(nameof(TariffDto.DeliveryCity), FieldType.Text),
                        new UserConfigurationGridColumnWhitchSource(nameof(TariffDto.TarifficationType), FieldType.Enum, nameof(TarifficationType)),
                        new UserConfigurationGridColumnWhitchSource(nameof(TariffDto.CarrierId), FieldType.Select, nameof(TransportCompaniesService)),
                        new UserConfigurationGridColumnWhitchSource(nameof(TariffDto.VehicleTypeId), FieldType.Select, nameof(VehicleTypesService)),
                        new UserConfigurationGridColumn(nameof(TariffDto.FtlRate), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate1), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate2), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate3), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate4), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate5), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate6), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate7), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate8), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate9), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate10), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate11), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate12), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate13), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate14), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate15), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate16), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate17), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate18), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate19), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate20), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate21), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate22), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate23), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate24), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate25), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate26), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate27), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate28), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate29), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate30), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate31), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate32), FieldType.Number),
                        new UserConfigurationGridColumn(nameof(TariffDto.LtlRate33), FieldType.Number),
                        /*end of add field for Tariffs*/
                    }
                });
            }

            if (_identityService.HasPermissions(RolePermissions.WarehousesEdit))
            {
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<WarehousesService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.WarehousesEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Warehouses*/
                            new UserConfigurationGridColumn(nameof(WarehouseDto.WarehouseName), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.SoldToNumber), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.Region), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.City), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.Address), FieldType.Text),
                            new UserConfigurationGridColumnWhitchSource(nameof(WarehouseDto.PickingTypeId), FieldType.Select, nameof(PickingTypesService)),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.LeadtimeDays), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.CustomerWarehouse), FieldType.Boolean),
                            /*end of add field for Warehouses*/
                        }
                });
            }

            if (_identityService.HasPermissions(RolePermissions.ArticlesEdit))
            {
                dicts.Add(
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<ArticlesService>(),
                        CanCreateByForm = _identityService.HasPermissions(RolePermissions.ArticlesEdit),
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Articles*/
                            new UserConfigurationGridColumn(nameof(ArticleDto.Spgr), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.Description), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.Nart), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.CountryOfOrigin), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.ShelfLife), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.Status), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.Ean), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.UnitLengthGoodsMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthUnitsGoodsMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.UnitHeightGoodsMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WeightUnitsGrossProductG), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WeightUnitsNetGoodsG), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.EanShrink), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesInShrink), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LengthShrinkMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthShrinkMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.HeightShrinkMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossShrinkWeightG), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetWeightShrinkG), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.EanBox), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesInABox), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.BoxLengthMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthOfABoxMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.BoxHeightMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossBoxWeightG), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetBoxWeightG), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesInALayer), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LayerLengthMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LayerWidthMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LayerHeightMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossLayerWeightMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetWeightMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.EanPallet), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesOnAPallet), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PalletLengthMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthOfPalletsMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PalletHeightMm), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossPalletWeightG), FieldType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetWeightPalletsG), FieldType.Number),
                            /*end of add field for Articles*/
                        }
                    }
                    );
            }

            if (_identityService.HasPermissions(RolePermissions.PickingTypesEdit))
            {
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<PickingTypesService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.PickingTypesEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn(nameof(PickingTypeDto.Name), FieldType.Text)
                        }
                });
            }

            if (_identityService.HasPermissions(RolePermissions.TransportCompaniesEdit))
            {
                dicts.Add(
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<TransportCompaniesService>(),
                        CanCreateByForm = _identityService.HasPermissions(RolePermissions.TransportCompaniesEdit),
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for TransportCompanies*/
                            new UserConfigurationGridColumn(nameof(TransportCompanyDto.Title), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(TransportCompanyDto.ContractNumber), FieldType.Text),
                            new UserConfigurationGridColumn(nameof(TransportCompanyDto.DateOfPowerOfAttorney), FieldType.Text),
                            /*end of add field for TransportCompanies*/
                        }
                });
            }

            if (_identityService.HasPermissions(RolePermissions.VehicleTypesEdit))
            {
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<VehicleTypesService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.VehicleTypesEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn(nameof(VehicleTypeDto.Name), FieldType.Text)
                        }
                });
            }

            if (_identityService.HasPermissions(RolePermissions.DocumentTypesEdit))
            {
                dicts.Add(new UserConfigurationDictionaryItem
                {
                    Name = GetName<DocumentTypesService>(),
                    CanCreateByForm = _identityService.HasPermissions(RolePermissions.DocumentTypesEdit),
                    CanExportToExcel = true,
                    CanImportFromExcel = true,
                    ShowOnHeader = false,
                    Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn(nameof(DocumentTypeDto.Name), FieldType.Text)
                        }
                });
            }
                
            return dicts;
        }

        private List<UserConfigurationGridColumn> ExtractColumnsFromDto<TDto>()
        {
            var props = typeof(TDto).GetProperties()
                                    .Where(prop => Attribute.IsDefined(prop, typeof(FieldTypeAttribute)))
                                    .OrderBy(prop => Attribute.IsDefined(prop, typeof(IsDefaultAttribute))
                                                    ? ((IsDefaultAttribute)Attribute.GetCustomAttribute(prop, typeof(IsDefaultAttribute))).OrderNumber
                                                    : int.MaxValue);

            var columns = new List<UserConfigurationGridColumn>();
            foreach (var prop in props)
            {
                var fieldType = (FieldTypeAttribute)Attribute.GetCustomAttribute(prop, typeof(FieldTypeAttribute));
                var isDefault = Attribute.IsDefined(prop, typeof(IsDefaultAttribute));
                var column = new UserConfigurationGridColumnWhitchSource(prop.Name, fieldType.Type, fieldType.Source, isDefault, fieldType.ShowRawValue);
                columns.Add(column);
            }

            return columns;
        }

    }
}