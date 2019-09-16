using System.Collections.Generic;
/*start of using application service*/
using Application.Services.Orders;
using Application.Services.Shippings;
using Application.Services.Tariffs;
using Application.Services.Warehouses;
using Application.Services.Articles;
using Application.Services.TransportСompanies;
/*end of using application service*/
using Domain.Enums;
using Domain.Services.AppConfiguration;

namespace Application.Services.AppConfiguration
{
    
    public class AppConfigurationService : IAppConfigurationService 
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
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Orders*/
                            new UserConfigurationGridColumn("SalesOrderNumber", FiledType.Text),
                            new UserConfigurationGridColumn("OrderDate", FiledType.Text),
                            new UserConfigurationGridColumn("TypeOfOrder", FiledType.Text),
                            new UserConfigurationGridColumn("Payer", FiledType.Text),
                            new UserConfigurationGridColumn("CustomerName", FiledType.Text),
                            new UserConfigurationGridColumn("SoldTo", FiledType.Text),
                            new UserConfigurationGridColumn("ShippingDate", FiledType.Text),
                            new UserConfigurationGridColumn("DaysOnTheRoad", FiledType.Text),
                            new UserConfigurationGridColumn("DeliveryDate", FiledType.Text),
                            new UserConfigurationGridColumn("BDFInvoiceNumber", FiledType.Text),
                            new UserConfigurationGridColumn("InvoiceNumber", FiledType.Text),
                            new UserConfigurationGridColumn("NumberOfArticles", FiledType.Text),
                            new UserConfigurationGridColumn("TheNumberOfBoxes", FiledType.Text),
                            new UserConfigurationGridColumn("PreliminaryNumberOfPallets", FiledType.Text),
                            new UserConfigurationGridColumn("ActualNumberOfPallets", FiledType.Text),
                            new UserConfigurationGridColumn("ConfirmedBoxes", FiledType.Text),
                            new UserConfigurationGridColumn("ConfirmedNumberOfPallets", FiledType.Text),
                            new UserConfigurationGridColumn("WeightKg", FiledType.Text),
                            new UserConfigurationGridColumn("OrderAmountExcludingVAT", FiledType.Text),
                            new UserConfigurationGridColumn("TTNAmountExcludingVAT", FiledType.Text),
                            new UserConfigurationGridColumn("Region", FiledType.Text),
                            new UserConfigurationGridColumn("City", FiledType.Text),
                            new UserConfigurationGridColumn("ShippingAddress", FiledType.Text),
                            new UserConfigurationGridColumn("DeliveryAddress", FiledType.Text),
                            new UserConfigurationGridColumn("CustomerAvizTime", FiledType.Text),
                            new UserConfigurationGridColumn("OrderComments", FiledType.Text),
                            new UserConfigurationGridColumn("TypeOfEquipment", FiledType.Text),
                            new UserConfigurationGridColumn("PlannedArrivalTimeSlotBDFWarehouse", FiledType.Text),
                            new UserConfigurationGridColumn("ArrivalTimeForLoadingBDFWarehouse", FiledType.Text),
                            new UserConfigurationGridColumn("DepartureTimeFromTheBDFWarehouse", FiledType.Text),
                            new UserConfigurationGridColumn("ActualDateOfArrivalAtTheConsignee", FiledType.Text),
                            new UserConfigurationGridColumn("ArrivalTimeToConsignee", FiledType.Text),
                            new UserConfigurationGridColumn("DateOfDepartureFromTheConsignee", FiledType.Text),
                            new UserConfigurationGridColumn("DepartureTimeFromConsignee", FiledType.Text),
                            new UserConfigurationGridColumn("TheNumberOfHoursOfDowntime", FiledType.Text),
                            new UserConfigurationGridColumn("ReturnInformation", FiledType.Text),
                            new UserConfigurationGridColumn("ReturnShippingAccountNo", FiledType.Text),
                            new UserConfigurationGridColumn("PlannedReturnDate", FiledType.Text),
                            new UserConfigurationGridColumn("ActualReturnDate", FiledType.Text),
                            new UserConfigurationGridColumn("MajorAdoptionNumber", FiledType.Text),
                            new UserConfigurationGridColumn("Avization", FiledType.Text),
                            new UserConfigurationGridColumn("Status", FiledType.Text),
                            new UserConfigurationGridColumn("OrderItems", FiledType.Text),
                            new UserConfigurationGridColumn("OrderCreationDate", FiledType.Text),
                            /*end of add field for Orders*/
                        }
                    },
                    new UserConfigurationGridItem
                    {
                        Name = GetName<ShippingsService>(), 
                        CanCreateByForm = true, 
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Shippings*/
                            new UserConfigurationGridColumn("TransportationNumber", FiledType.Text),
                            new UserConfigurationGridColumn("DeliveryMethod", FiledType.Text),
                            new UserConfigurationGridColumn("ThermalMode", FiledType.Text),
                            new UserConfigurationGridColumn("BillingMethod", FiledType.Text),
                            new UserConfigurationGridColumn("TransportCompany", FiledType.Text),
                            new UserConfigurationGridColumn("DeliveryInvoiceNumber", FiledType.Text),
                            new UserConfigurationGridColumn("CommentsReasonsForDeviationFromTheSchedule", FiledType.Text),
                            new UserConfigurationGridColumn("TransportationCostWithoutVAT", FiledType.Text),
                            new UserConfigurationGridColumn("ReturnShippingCostExcludingVAT", FiledType.Text),
                            new UserConfigurationGridColumn("AdditionalShippingCostsExcludingVAT", FiledType.Text),
                            new UserConfigurationGridColumn("AdditionalShippingCostsComments", FiledType.Text),
                            new UserConfigurationGridColumn("Waybill", FiledType.Text),
                            new UserConfigurationGridColumn("WaybillTorg12", FiledType.Text),
                            new UserConfigurationGridColumn("WaybillTransportSection", FiledType.Text),
                            new UserConfigurationGridColumn("Invoice", FiledType.Text),
                            new UserConfigurationGridColumn("DeliveryStatus", FiledType.Text),
                            new UserConfigurationGridColumn("AmountConfirmedByShipper", FiledType.Text),
                            new UserConfigurationGridColumn("AmountConfirmedByTC", FiledType.Text),
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
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for Tariffs*/
                            new UserConfigurationGridColumn("CityOfShipment", FiledType.Text),
                            new UserConfigurationGridColumn("DeliveryCity", FiledType.Text),
                            new UserConfigurationGridColumn("VehicleType", FiledType.Text),
                            new UserConfigurationGridColumn("FTLBet", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate1", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate2", FiledType.Text),
                            new UserConfigurationGridColumn("BetLTL3", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate4", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate5", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate6", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate7", FiledType.Text),
                            new UserConfigurationGridColumn("LTLBet8", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate9", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate10", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate11", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate12", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate13", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate14", FiledType.Text),
                            new UserConfigurationGridColumn("BetLTL15", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate16", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate17", FiledType.Text),
                            new UserConfigurationGridColumn("BetLTL18", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate19", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate20", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate21", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate22", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate23", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate24", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate25", FiledType.Text),
                            new UserConfigurationGridColumn("LTLBet26", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate27", FiledType.Text),
                            new UserConfigurationGridColumn("LTLBet28", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate29", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate30", FiledType.Text),
                            new UserConfigurationGridColumn("LTLRate31", FiledType.Text),
                            new UserConfigurationGridColumn("BetLTL32", FiledType.Text),
                            new UserConfigurationGridColumn("LTLBid33", FiledType.Text),
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
                            new UserConfigurationGridColumn("TheNameOfTheWarehouse", FiledType.Text),
                            new UserConfigurationGridColumn("SoldToNumber", FiledType.Text),
                            new UserConfigurationGridColumn("Address", FiledType.Text),
                            new UserConfigurationGridColumn("LeadtimeDays", FiledType.Text),
                            new UserConfigurationGridColumn("CustomerWarehouse", FiledType.Text),
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
                            new UserConfigurationGridColumn("SPGR", FiledType.Text),
                            new UserConfigurationGridColumn("Description", FiledType.Text),
                            new UserConfigurationGridColumn("Nart", FiledType.Text),
                            new UserConfigurationGridColumn("CountryOfOrigin", FiledType.Text),
                            new UserConfigurationGridColumn("ShelfLife", FiledType.Text),
                            new UserConfigurationGridColumn("Ean", FiledType.Text),
                            new UserConfigurationGridColumn("UnitLengthGoodsMm", FiledType.Text),
                            new UserConfigurationGridColumn("WidthUnitsGoodsMm", FiledType.Text),
                            new UserConfigurationGridColumn("UnitHeightGoodsMm", FiledType.Text),
                            new UserConfigurationGridColumn("WeightUnitsGrossProductG", FiledType.Text),
                            new UserConfigurationGridColumn("WeightUnitsNetGoodsG", FiledType.Text),
                            new UserConfigurationGridColumn("EANShrink", FiledType.Text),
                            new UserConfigurationGridColumn("PiecesInShrink", FiledType.Text),
                            new UserConfigurationGridColumn("LengthShrinkMm", FiledType.Text),
                            new UserConfigurationGridColumn("WidthShrinkMm", FiledType.Text),
                            new UserConfigurationGridColumn("HeightShrinkMm", FiledType.Text),
                            new UserConfigurationGridColumn("GrossShrinkWeightG", FiledType.Text),
                            new UserConfigurationGridColumn("NetWeightShrinkG", FiledType.Text),
                            new UserConfigurationGridColumn("EANBox", FiledType.Text),
                            new UserConfigurationGridColumn("PiecesInABox", FiledType.Text),
                            new UserConfigurationGridColumn("BoxLengthMm", FiledType.Text),
                            new UserConfigurationGridColumn("WidthOfABoxMm", FiledType.Text),
                            new UserConfigurationGridColumn("BoxHeightMm", FiledType.Text),
                            new UserConfigurationGridColumn("GrossBoxWeightG", FiledType.Text),
                            new UserConfigurationGridColumn("NetBoxWeightG", FiledType.Text),
                            new UserConfigurationGridColumn("PiecesInALayer", FiledType.Text),
                            new UserConfigurationGridColumn("LayerLengthMm", FiledType.Text),
                            new UserConfigurationGridColumn("LayerWidthMm", FiledType.Text),
                            new UserConfigurationGridColumn("LayerHeightMm", FiledType.Text),
                            new UserConfigurationGridColumn("GrossLayerWeightMm", FiledType.Text),
                            new UserConfigurationGridColumn("NetWeightMm", FiledType.Text),
                            new UserConfigurationGridColumn("EANPallet", FiledType.Text),
                            new UserConfigurationGridColumn("PiecesOnAPallet", FiledType.Text),
                            new UserConfigurationGridColumn("PalletLengthMm", FiledType.Text),
                            new UserConfigurationGridColumn("WidthOfPalletsMm", FiledType.Text),
                            new UserConfigurationGridColumn("PalletHeightMm", FiledType.Text),
                            new UserConfigurationGridColumn("GrossPalletWeightG", FiledType.Text),
                            new UserConfigurationGridColumn("NetWeightPalletsG", FiledType.Text),
                            /*end of add field for Articles*/
                        }
                    },
                    new UserConfigurationDictionaryItem
                    {
                        Name = GetName<TransportСompaniesService>(), 
                        CanCreateByForm = true, 
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            /*start of add field for TransportСompanies*/
                            new UserConfigurationGridColumn("Title", FiledType.Text),
                            new UserConfigurationGridColumn("ContractNumber", FiledType.Text),
                            new UserConfigurationGridColumn("DateOfPowerOfAttorney", FiledType.Text),
                            /*end of add field for TransportСompanies*/
                        }
                    },
                    /*end of add dictionaries*/
                }                
            };
        }

        private string GetName<T>()
        {
            return typeof(T).Name.Replace("Service", "");
        }
    }     
}