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
using Domain.Services.AppConfiguration;
/*start of using domain service*/
using Domain.Services.Orders;
using Domain.Services.Shippings;
using Domain.Services.Tariffs;
using Domain.Services.Warehouses;
using Domain.Services.Articles;
using Domain.Services.TransportCompanies;
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
                            new UserConfigurationGridColumn(nameof(OrderDto.SalesOrderNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderDate), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.TypeOfOrder), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.Payer), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.CustomerName), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.SoldTo), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ShippingDate), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DaysOnTheRoad), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DeliveryDate), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.BDFInvoiceNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.InvoiceNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.NumberOfArticles), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.TheNumberOfBoxes), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.PreliminaryNumberOfPallets), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ActualNumberOfPallets), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ConfirmedBoxes), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ConfirmedNumberOfPallets), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.WeightKg), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderAmountExcludingVAT), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.TTNAmountExcludingVAT), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.Region), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.City), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ShippingAddress), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DeliveryAddress), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.CustomerAvizTime), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderComments), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.TypeOfEquipment), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.PlannedArrivalTimeSlotBDFWarehouse), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ArrivalTimeForLoadingBDFWarehouse), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DepartureTimeFromTheBDFWarehouse), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ActualDateOfArrivalAtTheConsignee), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ArrivalTimeToConsignee), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DateOfDepartureFromTheConsignee), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.DepartureTimeFromConsignee), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.TheNumberOfHoursOfDowntime), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ReturnInformation), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ReturnShippingAccountNo), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.PlannedReturnDate), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ActualReturnDate), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.MajorAdoptionNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.Avization), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderItems), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.OrderCreationDate), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(OrderDto.ShippingId), FiledType.Select),
                            /*end of add field for Orders*/
                        }
                    },
                    new UserConfigurationGridItem
                    {
                        Name = GetName<ShippingsService>(), 
                        CanCreateByForm = true,
                        CanViewAdditionSummary = true,                        
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Shippings*/
                            new UserConfigurationGridColumn(nameof(ShippingDto.TransportationNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.DeliveryMethod), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ThermalMode), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.BillingMethod), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.TransportCompany), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.PreliminaryNumberOfPallets), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ActualNumberOfPallets), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ConfirmedNumberOfPallets), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.PlannedArrivalTimeSlotBDFWarehouse), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ArrivalTimeForLoadingBDFWarehouse), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.DepartureTimeFromTheBDFWarehouse), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.DeliveryInvoiceNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.CommentsReasonsForDeviationFromTheSchedule), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.TransportationCostWithoutVAT), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ReturnShippingCostExcludingVAT), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.AdditionalShippingCostsExcludingVAT), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.AdditionalShippingCostsComments), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.Waybill), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.WaybillTorg12), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.WaybillTransportSection), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.Invoice), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.ActualReturnDate), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.InvoiceNumber), FiledType.Text),
                            new UserConfigurationGridColumnWhitchSource(nameof(ShippingDto.Status), FiledType.State, nameof(ShippingState)),
                            new UserConfigurationGridColumn(nameof(ShippingDto.DeliveryStatus), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.AmountConfirmedByShipper), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ShippingDto.AmountConfirmedByTC), FiledType.Text),
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
                            new UserConfigurationGridColumn(nameof(WarehouseDto.TheNameOfTheWarehouse), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.SoldToNumber), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.Region), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.City), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.Address), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.TypeOfEquipment), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.LeadtimeDays), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(WarehouseDto.CustomerWarehouse), FiledType.Text),
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
                            new UserConfigurationGridColumn(nameof(ArticleDto.ShelfLife), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.Status), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.Ean), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.UnitLengthGoodsMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthUnitsGoodsMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.UnitHeightGoodsMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WeightUnitsGrossProductG), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WeightUnitsNetGoodsG), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.EANShrink), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesInShrink), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LengthShrinkMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthShrinkMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.HeightShrinkMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossShrinkWeightG), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetWeightShrinkG), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.EANBox), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesInABox), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.BoxLengthMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthOfABoxMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.BoxHeightMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossBoxWeightG), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetBoxWeightG), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesInALayer), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LayerLengthMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LayerWidthMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.LayerHeightMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossLayerWeightMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetWeightMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.EANPallet), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PiecesOnAPallet), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PalletLengthMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.WidthOfPalletsMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.PalletHeightMm), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.GrossPalletWeightG), FiledType.Text),
                            new UserConfigurationGridColumn(nameof(ArticleDto.NetWeightPalletsG), FiledType.Text),
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
                    /*end of add dictionaries*/
                }                
            };
        }
    }
}