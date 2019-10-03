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
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Orders*/
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.Status), FiledType.State, nameof(OrderState)),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderDate), FiledType.DateTime),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.OrderType), FiledType.Select, nameof(OrderType)),
                            new UserConfigurationGridColumn(nameof(OrderDto.Payer), FiledType.Text),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.ClientName), FiledType.Select, nameof(WarehousesService)),
                            new UserConfigurationGridColumn(nameof(OrderDto.SoldTo), FiledType.Text),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.PickingType), FiledType.Select, nameof(PickingTypesService)),
                            new UserConfigurationGridColumn(nameof(OrderDto.TemperatureMin), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.TemperatureMax), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.ShippingDate), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(OrderDto.DeliveryDate), FiledType.DateTime),
                            new UserConfigurationGridColumn(nameof(OrderDto.TransitDays), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(OrderDto.ShippingAddress), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DeliveryRegion), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DeliveryCity), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DeliveryAddress), FiledType.Text),
                            //new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.ShippingStatus), FiledType.State, nameof(VehicleState)),
                            //new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.DeliveryStatus), FiledType.State, nameof(VehicleState)),
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
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderCreationDate), FiledType.DateTime),
                            new UserConfigurationGridColumnWhitchSource(nameof(OrderDto.ShippingId), FiledType.Select, nameof(ShippingsService)),
                            /*end of add field for Orders*/
                        }
                    },
                    new UserConfigurationGridItem
                    {
                        Name = GetName<ShippingsService>(), 
                        CanCreateByForm = false,
                        CanViewAdditionSummary = true,                        
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Shippings*/
                            new UserConfigurationGridColumnWhitchSource(nameof(ShippingDto.Status), FiledType.State, nameof(ShippingState)),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ShippingNumber), FiledType.Text),
                            new UserConfigurationGridColumnWhitchSource(nameof(ShippingDto.DeliveryType), FiledType.Select, nameof(DeliveryType)),
                            new UserConfigurationGridColumn(nameof(ShippingDto.TemperatureMin), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ShippingDto.TemperatureMax), FiledType.Number),
                            new UserConfigurationGridColumnWhitchSource(nameof(ShippingDto.TarifficationType), FiledType.Select, nameof(TarifficationType)),
                            new UserConfigurationGridColumnWhitchSource(nameof(ShippingDto.Carrier), FiledType.Select, nameof(TransportCompaniesService)),
                            new UserConfigurationGridColumn(nameof(ShippingDto.VehicleType), FiledType.Text),
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
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<TranslationsService>(), 
                        CanCreateByForm = true, 
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            new UserConfigurationGridColumn("name", FiledType.Text),
                            new UserConfigurationGridColumn("ru", FiledType.Text),
                            new UserConfigurationGridColumn("en", FiledType.Text),
                        }
                    },
                    /*start of add dictionaries*/
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<TariffsService>(), 
                        CanCreateByForm = true, 
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Tariffs*/
                            new UserConfigurationGridColumn(nameof(TariffDto.CityOfShipment), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.DeliveryCity), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.BillingMethod), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.TransportCompany), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.VehicleType), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.FTLBet), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate1), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate2), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.BetLTL3), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate4), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate5), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate6), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate7), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLBet8), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate9), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate10), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate11), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate12), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate13), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate14), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.BetLTL15), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate16), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.BetLTL17), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.BetLTL18), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate19), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate20), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate21), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate22), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate23), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate24), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate25), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLBet26), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate27), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLBet28), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate29), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate30), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLRate31), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.BetLTL32), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(TariffDto.LTLBid33), FiledType.Text),
                            /*end of add field for Tariffs*/
                        }
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<WarehousesService>(), 
                        CanCreateByForm = true, 
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Warehouses*/
                            new UserConfigurationGridColumn(nameof(WarehouseDto.WarehouseName), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.SoldToNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.Region), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.City), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.Address), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.PickingType), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.LeadtimeDays), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.CustomerWarehouse), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.UsePickingType), FiledType.Text),
                            /*end of add field for Warehouses*/
                        }
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<ArticlesService>(), 
                        CanCreateByForm = true, 
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Articles*/
                            new UserConfigurationGridColumn(nameof(ArticleDto.SPGR), FiledType.Text),
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
                            new UserConfigurationGridColumn(nameof(ArticleDto.EANShrink), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesInShrink), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LengthShrinkMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthShrinkMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.HeightShrinkMm), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossShrinkWeightG), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetWeightShrinkG), FiledType.Number),
                            new UserConfigurationGridColumn(nameof(ArticleDto.EANBox), FiledType.Text),
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
                            new UserConfigurationGridColumn(nameof(ArticleDto.EANPallet), FiledType.Number),
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
                        CanImportFromExcel = true,
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
                        Name = GetName<DocumentTypesService>(),
                        CanCreateByForm = true,
                        CanImportFromExcel = true,
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