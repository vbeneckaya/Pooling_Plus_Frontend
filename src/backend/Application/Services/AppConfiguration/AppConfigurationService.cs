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
/*end of using domain service*/

namespace Application.Services.AppConfiguration
{
    
    public class AppConfigurationService : AppConfigurationServiceBase ,IAppConfigurationService 
    {
        
        public AppConfigurationService()
        {
        }
        
        public AppConfigurationDto GetConfiguration()
        {
            return new AppConfigurationDto
            {
                EditUsers = true,
                EditRoles = true,
                Grids = new List<UserConfigurationGridItem>
                {
                    /*start of add grids*/
                    new UserConfigurationGridItem
                    {
                        Name = GetName<OrdersService>(), 
                        CanCreateByForm = true,
                        CanViewAdditionSummary = true,
                        CanExportToExcel = true,
                        CanImportFromExcel = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Orders*/
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderNumber), FiledType.Text, isDefault: true),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.Status), FiledType.State, nameof(OrderState), isDefault: true),
                            new UserConfigurationGridColumn(nameof(OrderDto.ShippingNumber), FiledType.Text, isDefault: true),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.OrderShippingStatus), FiledType.State, nameof(ShippingState), isDefault: true),
                            new UserConfigurationGridColumn(nameof(OrderDto.ClientName), FiledType.Text, isDefault: true),
                            new UserConfigurationGridColumn(nameof(OrderDto.Payer), FiledType.Text, isDefault: true),
                            new UserConfigurationGridColumn(nameof(OrderDto.DeliveryDate), FiledType.DateTime, isDefault: true),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderCreationDate), FiledType.DateTime, isDefault: true),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderDate), FiledType.DateTime),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.OrderType), FiledType.Enum, nameof(OrderType)),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.SoldTo), FiledType.Select, nameof(SoldToService), showRawValue: true),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.PickingTypeId), FiledType.Select, nameof(PickingTypesService)),
                            new UserConfigurationGridColumn(nameof(OrderDto.TemperatureMin), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.TemperatureMax), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.ShippingDate), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(OrderDto.TransitDays), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.ShippingAddress), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DeliveryRegion), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DeliveryCity), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DeliveryAddress), FiledType.Text),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.ShippingStatus), FiledType.State, nameof(VehicleState)),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.DeliveryStatus), FiledType.State, nameof(VehicleState)),
                            new UserConfigurationGridColumn(nameof(OrderDto.ArticlesCount), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.BoxesCount), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.ConfirmedBoxesCount), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.PalletsCount), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.ConfirmedPalletsCount), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.ActualPalletsCount), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.WeightKg), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.ActualWeightKg), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderAmountExcludingVAT), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.BDFInvoiceNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.LoadingArrivalTime), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(OrderDto.LoadingDepartureTime), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(OrderDto.UnloadingArrivalDate), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(OrderDto.UnloadingArrivalTime), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.UnloadingDepartureDate), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(OrderDto.UnloadingDepartureTime), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.TrucksDowntime), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.ReturnInformation), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ReturnShippingAccountNo), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.PlannedReturnDate), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(OrderDto.ActualReturnDate), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(OrderDto.MajorAdoptionNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ClientAvisationTime), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderComments), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.WaybillTorg12), FiledType.Boolean),
                            new UserConfigurationGridColumn(nameof(OrderDto.Invoice), FiledType.Boolean),
                            new UserConfigurationGridColumn(nameof(OrderDto.DocumentsReturnDate), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(OrderDto.ActualDocumentsReturnDate), FiledType.DateTime),
                            /*end of add field for Orders*/
                        }
                    },
                    new UserConfigurationGridItem
                    {
                        Name = GetName<ShippingsService>(), 
                        CanCreateByForm = false,
                        CanViewAdditionSummary = true,
                        CanExportToExcel = true,
                        CanImportFromExcel = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Shippings*/
                            new UserConfigurationGridColumn(nameof(ShippingDto.ShippingNumber), FiledType.Text, isDefault: true),
                            new UserConfigurationGridColumnWhitchSource(nameof(ShippingDto.Status), FiledType.State, nameof(ShippingState), isDefault: true),
                            new UserConfigurationGridColumnWhitchSource(nameof(ShippingDto.CarrierId), FiledType.Select, nameof(TransportCompaniesService), isDefault: true),
                            new UserConfigurationGridColumnWhitchSource(nameof(ShippingDto.DeliveryType), FiledType.Enum, nameof(DeliveryType), isDefault: true),
                            new UserConfigurationGridColumnWhitchSource(nameof(ShippingDto.TarifficationType), FiledType.Enum, nameof(TarifficationType), isDefault: true),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ShippingCreationDate), FiledType.DateTime, isDefault: true),
                            new UserConfigurationGridColumn(nameof(ShippingDto.TemperatureMin), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.TemperatureMax), FiledType.Number),
                            new UserConfigurationGridColumnWhitchSource(nameof(ShippingDto.VehicleTypeId), FiledType.Select, nameof(VehicleTypesService)),
                            new UserConfigurationGridColumn(nameof(ShippingDto.PalletsCount), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ActualPalletsCount), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ConfirmedPalletsCount), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.WeightKg), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ActualWeightKg), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.LoadingArrivalTime), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(ShippingDto.LoadingDepartureTime), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(ShippingDto.DeliveryInvoiceNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.DeviationReasonsComments), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.TotalDeliveryCost), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.OtherCosts), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.DeliveryCostWithoutVAT), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ReturnCostWithoutVAT), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.InvoiceAmountWithoutVAT), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.AdditionalCostsWithoutVAT), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.AdditionalCostsComments), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.TrucksDowntime), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ReturnRate), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.AdditionalPointRate), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.DowntimeRate), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.BlankArrivalRate), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.BlankArrival), FiledType.Boolean),
                            new UserConfigurationGridColumn(nameof(ShippingDto.Waybill), FiledType.Boolean),
                            new UserConfigurationGridColumn(nameof(ShippingDto.WaybillTorg12), FiledType.Boolean),
                            new UserConfigurationGridColumn(nameof(ShippingDto.TransportWaybill), FiledType.Boolean),
                            new UserConfigurationGridColumn(nameof(ShippingDto.Invoice), FiledType.Boolean),
                            new UserConfigurationGridColumn(nameof(ShippingDto.DocumentsReturnDate), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ActualDocumentsReturnDate), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(ShippingDto.InvoiceNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.CostsConfirmedByShipper), FiledType.Boolean),
                            new UserConfigurationGridColumn(nameof(ShippingDto.CostsConfirmedByCarrier), FiledType.Boolean),
                            /*end of add field for Shippings*/
                        }
                    },
                    /*end of add grids*/
                }, 
                Dictionaries = new List<UserConfigurationDictionaryItem>
                {
                    /*start of add dictionaries*/
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<TariffsService>(),
                        CanCreateByForm = true,
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Tariffs*/
                            new UserConfigurationGridColumn(nameof(TariffDto.ShipmentCity), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.DeliveryCity), FiledType.Text),
                            new UserConfigurationGridColumnWhitchSource(nameof(TariffDto.TarifficationType), FiledType.Enum, nameof(TarifficationType)),
                            new UserConfigurationGridColumnWhitchSource(nameof(TariffDto.CarrierId), FiledType.Select, nameof(TransportCompaniesService)),
                            new UserConfigurationGridColumnWhitchSource(nameof(TariffDto.VehicleTypeId), FiledType.Select, nameof(VehicleTypesService)),
                            new UserConfigurationGridColumn(nameof(TariffDto.FtlRate), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate1), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate2), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate3), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate4), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate5), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate6), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate7), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate8), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate9), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate10), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate11), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate12), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate13), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate14), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate15), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate16), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate17), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate18), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate19), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate20), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate21), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate22), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate23), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate24), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate25), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate26), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate27), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate28), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate29), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate30), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate31), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate32), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(TariffDto.LtlRate33), FiledType.Number),
                            /*end of add field for Tariffs*/
                        }
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<WarehousesService>(),
                        CanCreateByForm = true,
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Warehouses*/
                            new UserConfigurationGridColumn(nameof(WarehouseDto.WarehouseName), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.SoldToNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.Region), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.City), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.Address), FiledType.Text),
                            new UserConfigurationGridColumnWhitchSource(nameof(WarehouseDto.PickingTypeId), FiledType.Select, nameof(PickingTypesService)),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.LeadtimeDays), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.CustomerWarehouse), FiledType.Boolean),
                            /*end of add field for Warehouses*/
                        }
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<ArticlesService>(),
                        CanCreateByForm = true,
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Articles*/
                            new UserConfigurationGridColumn(nameof(ArticleDto.Spgr), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.Description), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.Nart), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.CountryOfOrigin), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.ShelfLife), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.Status), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.Ean), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.UnitLengthGoodsMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthUnitsGoodsMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.UnitHeightGoodsMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WeightUnitsGrossProductG), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WeightUnitsNetGoodsG), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.EanShrink), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesInShrink), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LengthShrinkMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthShrinkMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.HeightShrinkMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossShrinkWeightG), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetWeightShrinkG), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.EanBox), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesInABox), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.BoxLengthMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthOfABoxMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.BoxHeightMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossBoxWeightG), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetBoxWeightG), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesInALayer), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LayerLengthMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LayerWidthMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LayerHeightMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossLayerWeightMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetWeightMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.EanPallet), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesOnAPallet), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PalletLengthMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthOfPalletsMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PalletHeightMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossPalletWeightG), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetWeightPalletsG), FiledType.Number),
                            /*end of add field for Articles*/
                        }
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<TransportCompaniesService>(),
                        CanCreateByForm = true,
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for TransportCompanies*/
                            new UserConfigurationGridColumn(nameof(TransportCompanyDto.Title), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TransportCompanyDto.ContractNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TransportCompanyDto.DateOfPowerOfAttorney), FiledType.Text),
                            /*end of add field for TransportCompanies*/
                        }
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<PickingTypesService>(),
                        CanCreateByForm = true,
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn(nameof(PickingTypeDto.Name), FiledType.Text)
                        }
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<VehicleTypesService>(),
                        CanCreateByForm = true,
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn(nameof(VehicleTypeDto.Name), FiledType.Text)
                        }
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<DocumentTypesService>(),
                        CanCreateByForm = true,
                        CanExportToExcel = true,
                        CanImportFromExcel = true,
                        ShowOnHeader = false,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn(nameof(DocumentTypeDto.Name), FiledType.Text)
                        }
                    },
                    /*end of add dictionaries*/
                }                
            };
        }
    }
}