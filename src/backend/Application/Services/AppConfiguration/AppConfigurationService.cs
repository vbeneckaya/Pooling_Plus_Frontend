using System.Collections.Generic;
/*start of using application service*/
using Application.Services.Orders;
using Application.Services.Shippings;
using Application.Services.Tariffs;
using Application.Services.Warehouses;
using Application.Services.Articles;
using Application.Services.Translations;
using Application.Services.TransportCompanies;
/*end of using application service*/
using Domain.Enums;
using Domain.Services.AppConfiguration;
using Domain.Services.Orders;

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
                        CanImportFromExcel = true,
                        Columns = new List<UserConfigurationGridColumn>
                        {
                            
                            /*start of add field for Orders*/
                            new UserConfigurationGridColumn(nameof(OrderDto.Status), FiledType.Text),
                            new UserConfigurationGridColumn("salesOrderNumber", FiledType.Text),
                            new UserConfigurationGridColumn("orderDate", FiledType.Text),
                            new UserConfigurationGridColumn("typeOfOrder", FiledType.Text),
                            new UserConfigurationGridColumn("payer", FiledType.Text),
                            new UserConfigurationGridColumn("customerName", FiledType.Text),
                            new UserConfigurationGridColumn("soldTo", FiledType.Text),
                            new UserConfigurationGridColumn("shippingDate", FiledType.Text),
                            new UserConfigurationGridColumn("daysOnTheRoad", FiledType.Text),
                            new UserConfigurationGridColumn("deliveryDate", FiledType.Text),
                            new UserConfigurationGridColumn("bDFInvoiceNumber", FiledType.Text),
                            new UserConfigurationGridColumn("invoiceNumber", FiledType.Text),
                            new UserConfigurationGridColumn("numberOfArticles", FiledType.Text),
                            new UserConfigurationGridColumn("theNumberOfBoxes", FiledType.Text),
                            new UserConfigurationGridColumn("preliminaryNumberOfPallets", FiledType.Text),
                            new UserConfigurationGridColumn("actualNumberOfPallets", FiledType.Text),
                            new UserConfigurationGridColumn("confirmedBoxes", FiledType.Text),
                            new UserConfigurationGridColumn("confirmedNumberOfPallets", FiledType.Text),
                            new UserConfigurationGridColumn("weightKg", FiledType.Text),
                            new UserConfigurationGridColumn("orderAmountExcludingVAT", FiledType.Text),
                            new UserConfigurationGridColumn("tTNAmountExcludingVAT", FiledType.Text),
                            new UserConfigurationGridColumn("region", FiledType.Text),
                            new UserConfigurationGridColumn("city", FiledType.Text),
                            new UserConfigurationGridColumn("shippingAddress", FiledType.Text),
                            new UserConfigurationGridColumn("deliveryAddress", FiledType.Text),
                            new UserConfigurationGridColumn("customerAvizTime", FiledType.Text),
                            new UserConfigurationGridColumn("orderComments", FiledType.Text),
                            new UserConfigurationGridColumn("typeOfEquipment", FiledType.Text),
                            new UserConfigurationGridColumn("plannedArrivalTimeSlotBDFWarehouse", FiledType.Text),
                            new UserConfigurationGridColumn("arrivalTimeForLoadingBDFWarehouse", FiledType.Text),
                            new UserConfigurationGridColumn("departureTimeFromTheBDFWarehouse", FiledType.Text),
                            new UserConfigurationGridColumn("actualDateOfArrivalAtTheConsignee", FiledType.Text),
                            new UserConfigurationGridColumn("arrivalTimeToConsignee", FiledType.Text),
                            new UserConfigurationGridColumn("dateOfDepartureFromTheConsignee", FiledType.Text),
                            new UserConfigurationGridColumn("departureTimeFromConsignee", FiledType.Text),
                            new UserConfigurationGridColumn("theNumberOfHoursOfDowntime", FiledType.Text),
                            new UserConfigurationGridColumn("returnInformation", FiledType.Text),
                            new UserConfigurationGridColumn("returnShippingAccountNo", FiledType.Text),
                            new UserConfigurationGridColumn("plannedReturnDate", FiledType.Text),
                            new UserConfigurationGridColumn("actualReturnDate", FiledType.Text),
                            new UserConfigurationGridColumn("majorAdoptionNumber", FiledType.Text),
                            new UserConfigurationGridColumn("avization", FiledType.Text),
                            new UserConfigurationGridColumn("orderItems", FiledType.Text),
                            new UserConfigurationGridColumn("orderCreationDate", FiledType.Text),
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
                            new UserConfigurationGridColumn("transportationNumber", FiledType.Text),
                            new UserConfigurationGridColumn("deliveryMethod", FiledType.Text),
                            new UserConfigurationGridColumn("thermalMode", FiledType.Text),
                            new UserConfigurationGridColumn("billingMethod", FiledType.Text),
                            new UserConfigurationGridColumn("transportCompany", FiledType.Text),
                            new UserConfigurationGridColumn("deliveryInvoiceNumber", FiledType.Text),
                            new UserConfigurationGridColumn("commentsReasonsForDeviationFromTheSchedule", FiledType.Text),
                            new UserConfigurationGridColumn("transportationCostWithoutVAT", FiledType.Text),
                            new UserConfigurationGridColumn("returnShippingCostExcludingVAT", FiledType.Text),
                            new UserConfigurationGridColumn("additionalShippingCostsExcludingVAT", FiledType.Text),
                            new UserConfigurationGridColumn("additionalShippingCostsComments", FiledType.Text),
                            new UserConfigurationGridColumn("waybill", FiledType.Text),
                            new UserConfigurationGridColumn("waybillTorg12", FiledType.Text),
                            new UserConfigurationGridColumn("waybillTransportSection", FiledType.Text),
                            new UserConfigurationGridColumn("invoice", FiledType.Text),
                            new UserConfigurationGridColumn("deliveryStatus", FiledType.Text),
                            new UserConfigurationGridColumn("amountConfirmedByShipper", FiledType.Text),
                            new UserConfigurationGridColumn("amountConfirmedByTC", FiledType.Text),
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
                            new UserConfigurationGridColumn("cityOfShipment", FiledType.Text),
                            new UserConfigurationGridColumn("deliveryCity", FiledType.Text),
                            new UserConfigurationGridColumn("vehicleType", FiledType.Text),
                            new UserConfigurationGridColumn("fTLBet", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate1", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate2", FiledType.Text),
                            new UserConfigurationGridColumn("betLTL3", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate4", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate5", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate6", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate7", FiledType.Text),
                            new UserConfigurationGridColumn("lTLBet8", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate9", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate10", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate11", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate12", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate13", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate14", FiledType.Text),
                            new UserConfigurationGridColumn("betLTL15", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate16", FiledType.Text),
                            new UserConfigurationGridColumn("betLTL17", FiledType.Text),
                            new UserConfigurationGridColumn("betLTL18", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate19", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate20", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate21", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate22", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate23", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate24", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate25", FiledType.Text),
                            new UserConfigurationGridColumn("lTLBet26", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate27", FiledType.Text),
                            new UserConfigurationGridColumn("lTLBet28", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate29", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate30", FiledType.Text),
                            new UserConfigurationGridColumn("lTLRate31", FiledType.Text),
                            new UserConfigurationGridColumn("betLTL32", FiledType.Text),
                            new UserConfigurationGridColumn("lTLBid33", FiledType.Text),
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
                            new UserConfigurationGridColumn("theNameOfTheWarehouse", FiledType.Text),
                            new UserConfigurationGridColumn("soldToNumber", FiledType.Text),
                            new UserConfigurationGridColumn("region", FiledType.Text),
                            new UserConfigurationGridColumn("city", FiledType.Text),
                            new UserConfigurationGridColumn("address", FiledType.Text),
                            new UserConfigurationGridColumn("typeOfEquipment", FiledType.Text),
                            new UserConfigurationGridColumn("leadtimeDays", FiledType.Text),
                            new UserConfigurationGridColumn("customerWarehouse", FiledType.Text),
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
                            new UserConfigurationGridColumn("sPGR", FiledType.Text),
                            new UserConfigurationGridColumn("description", FiledType.Text),
                            new UserConfigurationGridColumn("nart", FiledType.Text),
                            new UserConfigurationGridColumn("countryOfOrigin", FiledType.Text),
                            new UserConfigurationGridColumn("shelfLife", FiledType.Text),
                            new UserConfigurationGridColumn("ean", FiledType.Text),
                            new UserConfigurationGridColumn("unitLengthGoodsMm", FiledType.Text),
                            new UserConfigurationGridColumn("widthUnitsGoodsMm", FiledType.Text),
                            new UserConfigurationGridColumn("unitHeightGoodsMm", FiledType.Text),
                            new UserConfigurationGridColumn("weightUnitsGrossProductG", FiledType.Text),
                            new UserConfigurationGridColumn("weightUnitsNetGoodsG", FiledType.Text),
                            new UserConfigurationGridColumn("eANShrink", FiledType.Text),
                            new UserConfigurationGridColumn("piecesInShrink", FiledType.Text),
                            new UserConfigurationGridColumn("lengthShrinkMm", FiledType.Text),
                            new UserConfigurationGridColumn("widthShrinkMm", FiledType.Text),
                            new UserConfigurationGridColumn("heightShrinkMm", FiledType.Text),
                            new UserConfigurationGridColumn("grossShrinkWeightG", FiledType.Text),
                            new UserConfigurationGridColumn("netWeightShrinkG", FiledType.Text),
                            new UserConfigurationGridColumn("eANBox", FiledType.Text),
                            new UserConfigurationGridColumn("piecesInABox", FiledType.Text),
                            new UserConfigurationGridColumn("boxLengthMm", FiledType.Text),
                            new UserConfigurationGridColumn("widthOfABoxMm", FiledType.Text),
                            new UserConfigurationGridColumn("boxHeightMm", FiledType.Text),
                            new UserConfigurationGridColumn("grossBoxWeightG", FiledType.Text),
                            new UserConfigurationGridColumn("netBoxWeightG", FiledType.Text),
                            new UserConfigurationGridColumn("piecesInALayer", FiledType.Text),
                            new UserConfigurationGridColumn("layerLengthMm", FiledType.Text),
                            new UserConfigurationGridColumn("layerWidthMm", FiledType.Text),
                            new UserConfigurationGridColumn("layerHeightMm", FiledType.Text),
                            new UserConfigurationGridColumn("grossLayerWeightMm", FiledType.Text),
                            new UserConfigurationGridColumn("netWeightMm", FiledType.Text),
                            new UserConfigurationGridColumn("eANPallet", FiledType.Text),
                            new UserConfigurationGridColumn("piecesOnAPallet", FiledType.Text),
                            new UserConfigurationGridColumn("palletLengthMm", FiledType.Text),
                            new UserConfigurationGridColumn("widthOfPalletsMm", FiledType.Text),
                            new UserConfigurationGridColumn("palletHeightMm", FiledType.Text),
                            new UserConfigurationGridColumn("grossPalletWeightG", FiledType.Text),
                            new UserConfigurationGridColumn("netWeightPalletsG", FiledType.Text),
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
                            new UserConfigurationGridColumn("title", FiledType.Text),
                            new UserConfigurationGridColumn("contractNumber", FiledType.Text),
                            new UserConfigurationGridColumn("dateOfPowerOfAttorney", FiledType.Text),
                            /*end of add field for TransportCompanies*/
                        }
                    },
                    /*end of add dictionaries*/
                }                
            };
        }
    }
}