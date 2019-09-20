using System;
using System.Data;
using Domain.Extensions;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(1)]
    public class Migration001 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Roles",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String.WithSize(100)));
            Database.AddIndex("roles_pk", true, "Roles", "Id");
            
            var administratorRoleId = AddRole("Administrator");
            var transportСoordinatorId = AddRole("TransportCoordinator");
            var transportCompanyEmployee = AddRole("TransportCompanyEmployee");

            Database.AddTable("Users",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String.WithSize(100)),
                new Column("RoleId", DbType.Guid),
                new Column("Email", DbType.String.WithSize(100)),
                new Column("IsActive", DbType.Boolean),
                new Column("FieldsConfig", DbType.String.WithSize(300)),
                new Column("PasswordHash", DbType.String.WithSize(300)));
            
            Database.AddIndex("users_pk", true, "Users", "Id");
            
            AddUser("Иван Иванов", administratorRoleId, "admin@admin.ru", "123".GetHash());
            AddUser("Максим Координатович", transportСoordinatorId, "max@tms.ru", "123".GetHash());
            AddUser("Сергей Газельев", transportCompanyEmployee, "gazelev@tms.ru", "123".GetHash());
            
            Database.AddTable("Translations",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String.WithSize(100)),
                new Column("En", DbType.String.WithSize(100)),
                new Column("Ru", DbType.String.WithSize(100))
                );
            Database.AddIndex("translations_pk", true, "Translations", "Id");


            /*start of add tables*/
            Database.AddTable("Orders",
                new Column("Status", DbType.String),
                new Column("SalesOrderNumber", DbType.String),
                new Column("OrderDate", DbType.String),
                new Column("TypeOfOrder", DbType.String),
                new Column("Payer", DbType.String),
                new Column("CustomerName", DbType.String),
                new Column("SoldTo", DbType.String),
                new Column("ShippingDate", DbType.String),
                new Column("DaysOnTheRoad", DbType.String),
                new Column("DeliveryDate", DbType.String),
                new Column("BDFInvoiceNumber", DbType.String),
                new Column("InvoiceNumber", DbType.String),
                new Column("NumberOfArticles", DbType.String),
                new Column("TheNumberOfBoxes", DbType.String),
                new Column("PreliminaryNumberOfPallets", DbType.String),
                new Column("ActualNumberOfPallets", DbType.String),
                new Column("ConfirmedBoxes", DbType.String),
                new Column("ConfirmedNumberOfPallets", DbType.String),
                new Column("WeightKg", DbType.String),
                new Column("OrderAmountExcludingVAT", DbType.String),
                new Column("TTNAmountExcludingVAT", DbType.String),
                new Column("Region", DbType.String),
                new Column("City", DbType.String),
                new Column("ShippingAddress", DbType.String),
                new Column("DeliveryAddress", DbType.String),
                new Column("CustomerAvizTime", DbType.String),
                new Column("OrderComments", DbType.String),
                new Column("TypeOfEquipment", DbType.String),
                new Column("PlannedArrivalTimeSlotBDFWarehouse", DbType.String),
                new Column("ArrivalTimeForLoadingBDFWarehouse", DbType.String),
                new Column("DepartureTimeFromTheBDFWarehouse", DbType.String),
                new Column("ActualDateOfArrivalAtTheConsignee", DbType.String),
                new Column("ArrivalTimeToConsignee", DbType.String),
                new Column("DateOfDepartureFromTheConsignee", DbType.String),
                new Column("DepartureTimeFromConsignee", DbType.String),
                new Column("TheNumberOfHoursOfDowntime", DbType.String),
                new Column("ReturnInformation", DbType.String),
                new Column("ReturnShippingAccountNo", DbType.String),
                new Column("PlannedReturnDate", DbType.String),
                new Column("ActualReturnDate", DbType.String),
                new Column("MajorAdoptionNumber", DbType.String),
                new Column("Avization", DbType.String),
                new Column("OrderItems", DbType.String),
                new Column("OrderCreationDate", DbType.String),
                /*general fields for Orders*/
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey)
            );
            Database.AddIndex("orders_pk", true, "Orders", "Id");

            Database.AddTable("Shippings",
                new Column("TransportationNumber", DbType.String),
                new Column("DeliveryMethod", DbType.String),
                new Column("ThermalMode", DbType.String),
                new Column("BillingMethod", DbType.String),
                new Column("TransportCompany", DbType.String),
                new Column("PreliminaryNumberOfPallets", DbType.String),
                new Column("ActualNumberOfPallets", DbType.String),
                new Column("ConfirmedNumberOfPallets", DbType.String),
                new Column("PlannedArrivalTimeSlotBDFWarehouse", DbType.String),
                new Column("ArrivalTimeForLoadingBDFWarehouse", DbType.String),
                new Column("DepartureTimeFromTheBDFWarehouse", DbType.String),
                new Column("DeliveryInvoiceNumber", DbType.String),
                new Column("CommentsReasonsForDeviationFromTheSchedule", DbType.String),
                new Column("TransportationCostWithoutVAT", DbType.String),
                new Column("ReturnShippingCostExcludingVAT", DbType.String),
                new Column("AdditionalShippingCostsExcludingVAT", DbType.String),
                new Column("AdditionalShippingCostsComments", DbType.String),
                new Column("Waybill", DbType.String),
                new Column("WaybillTorg12", DbType.String),
                new Column("WaybillTransportSection", DbType.String),
                new Column("Invoice", DbType.String),
                new Column("ActualReturnDate", DbType.String),
                new Column("InvoiceNumber", DbType.String),
                new Column("Status", DbType.String),
                new Column("DeliveryStatus", DbType.String),
                new Column("AmountConfirmedByShipper", DbType.String),
                new Column("AmountConfirmedByTC", DbType.String),
                /*general fields for Shippings*/
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey)
            );
            Database.AddIndex("shippings_pk", true, "Shippings", "Id");

            Database.AddTable("Tariffs",
                new Column("CityOfShipment", DbType.String),
                new Column("DeliveryCity", DbType.String),
                new Column("BillingMethod", DbType.String),
                new Column("TransportCompany", DbType.String),
                new Column("VehicleType", DbType.String),
                new Column("FTLBet", DbType.String),
                new Column("LTLRate1", DbType.String),
                new Column("LTLRate2", DbType.String),
                new Column("BetLTL3", DbType.String),
                new Column("LTLRate4", DbType.String),
                new Column("LTLRate5", DbType.String),
                new Column("LTLRate6", DbType.String),
                new Column("LTLRate7", DbType.String),
                new Column("LTLBet8", DbType.String),
                new Column("LTLRate9", DbType.String),
                new Column("LTLRate10", DbType.String),
                new Column("LTLRate11", DbType.String),
                new Column("LTLRate12", DbType.String),
                new Column("LTLRate13", DbType.String),
                new Column("LTLRate14", DbType.String),
                new Column("BetLTL15", DbType.String),
                new Column("LTLRate16", DbType.String),
                new Column("BetLTL17", DbType.String),
                new Column("BetLTL18", DbType.String),
                new Column("LTLRate19", DbType.String),
                new Column("LTLRate20", DbType.String),
                new Column("LTLRate21", DbType.String),
                new Column("LTLRate22", DbType.String),
                new Column("LTLRate23", DbType.String),
                new Column("LTLRate24", DbType.String),
                new Column("LTLRate25", DbType.String),
                new Column("LTLBet26", DbType.String),
                new Column("LTLRate27", DbType.String),
                new Column("LTLBet28", DbType.String),
                new Column("LTLRate29", DbType.String),
                new Column("LTLRate30", DbType.String),
                new Column("LTLRate31", DbType.String),
                new Column("BetLTL32", DbType.String),
                new Column("LTLBid33", DbType.String),
                /*general fields for Tariffs*/
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey)
            );
            Database.AddIndex("tariffs_pk", true, "Tariffs", "Id");

            Database.AddTable("Warehouses",
                new Column("TheNameOfTheWarehouse", DbType.String),
                new Column("SoldToNumber", DbType.String),
                new Column("Address", DbType.String),
                new Column("LeadtimeDays", DbType.String),
                new Column("CustomerWarehouse", DbType.String),
                /*general fields for Warehouses*/
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey)
            );
            Database.AddIndex("warehouses_pk", true, "Warehouses", "Id");

            Database.AddTable("Articles",
                new Column("SPGR", DbType.String),
                new Column("Description", DbType.String),
                new Column("Nart", DbType.String),
                new Column("CountryOfOrigin", DbType.String),
                new Column("ShelfLife", DbType.String),
                new Column("Ean", DbType.String),
                new Column("UnitLengthGoodsMm", DbType.String),
                new Column("WidthUnitsGoodsMm", DbType.String),
                new Column("UnitHeightGoodsMm", DbType.String),
                new Column("WeightUnitsGrossProductG", DbType.String),
                new Column("WeightUnitsNetGoodsG", DbType.String),
                new Column("EANShrink", DbType.String),
                new Column("PiecesInShrink", DbType.String),
                new Column("LengthShrinkMm", DbType.String),
                new Column("WidthShrinkMm", DbType.String),
                new Column("HeightShrinkMm", DbType.String),
                new Column("GrossShrinkWeightG", DbType.String),
                new Column("NetWeightShrinkG", DbType.String),
                new Column("EANBox", DbType.String),
                new Column("PiecesInABox", DbType.String),
                new Column("BoxLengthMm", DbType.String),
                new Column("WidthOfABoxMm", DbType.String),
                new Column("BoxHeightMm", DbType.String),
                new Column("GrossBoxWeightG", DbType.String),
                new Column("NetBoxWeightG", DbType.String),
                new Column("PiecesInALayer", DbType.String),
                new Column("LayerLengthMm", DbType.String),
                new Column("LayerWidthMm", DbType.String),
                new Column("LayerHeightMm", DbType.String),
                new Column("GrossLayerWeightMm", DbType.String),
                new Column("NetWeightMm", DbType.String),
                new Column("EANPallet", DbType.String),
                new Column("PiecesOnAPallet", DbType.String),
                new Column("PalletLengthMm", DbType.String),
                new Column("WidthOfPalletsMm", DbType.String),
                new Column("PalletHeightMm", DbType.String),
                new Column("GrossPalletWeightG", DbType.String),
                new Column("NetWeightPalletsG", DbType.String),
                /*general fields for Articles*/
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey)
            );
            Database.AddIndex("articles_pk", true, "Articles", "Id");

            Database.AddTable("TransportCompanies",
                new Column("Title", DbType.String),
                new Column("ContractNumber", DbType.String),
                new Column("DateOfPowerOfAttorney", DbType.String),
                /*general fields for TransportCompanies*/
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey)
            );
            Database.AddIndex("transportCompanies_pk", true, "TransportCompanies", "Id");

            /*end of add tables*/
            
            AddTranslation("UserNotFound", "User not found", "Пользователь не найден или не активен");
            AddTranslation("UserIncorrectData", "The username or password you entered is incorrect", "Неверное имя пользователя или пароль");
            AddTranslation("Users", "Users", "Пользователи");
            AddTranslation("Roles", "Roles", "Роли");
            AddTranslation("isActive", "IsActive", "Активен");

            AddTranslation("login_btn", "Login", "Войти");
            AddTranslation("password", "password", "Пароль");
            AddTranslation("login", "email", "email");
            AddTranslation("dictionaries", "Dictionaries", "Справочники");
            AddTranslation("users", "Users", "Пользователи");
            AddTranslation("roles", "Roles", "Роли");
            AddTranslation("create_btn", "Create", "Создать");
            AddTranslation("search_all_fields", "Search all fields", "Искать по всем полям");
            AddTranslation("reset_filters", "Reset filters", "Сбросить фильтры");
            AddTranslation("exit", "Exit", "Выйти");
            AddTranslation("translations", "Translations", "Локализация");            
            AddTranslation("Administrator", "Administrator", "Администратор");
            AddTranslation("TransportCoordinator", "Transport Coordinator", "Транспортный координатор");
            AddTranslation("TransportCompanyEmployee", "Transport Company Employee", "Сотрудник транспортной компании");
            AddTranslation("permissions", "permissions", "Разрешения");
            AddTranslation("name", "Name", "Наименование");
            AddTranslation("userName", "User name", "ФИО");
            
            /*start of add translates for action*/
            AddTranslation("createShipping", "Create shipping", "Создать перевозку");
            AddTranslation("unionOrders", "Union orders", "Объеденить в перевозку");
            /*start of add translates for action*/
            
            /*start of add translates*/
            AddTranslation("login_welcome", "login_welcome", "TMS Beiersdorf");
            AddTranslation("login_support", "login_support", "TMS для компании Beiersdorf");
            AddTranslation("order", "Order", "Заказ");
            AddTranslation("orders", "Orders", "Заказы");
            AddTranslation("status", "Status", "Статус");
            AddTranslation("salesOrderNumber", "SalesOrderNumber", "Номер заказ клиента");
            AddTranslation("orderDate", "OrderDate", "Дата заказа");
            AddTranslation("typeOfOrder", "TypeOfOrder", "Тип заказа");
            AddTranslation("payer", "Payer", "Плательщик");
            AddTranslation("customerName", "CustomerName", "Название клиента");
            AddTranslation("soldTo", "SoldTo", "Sold-to");
            AddTranslation("shippingDate", "ShippingDate", "Дата отгрузки");
            AddTranslation("daysOnTheRoad", "DaysOnTheRoad", "Дней в пути");
            AddTranslation("deliveryDate", "DeliveryDate", "Дата доставки");
            AddTranslation("bDFInvoiceNumber", "BDFInvoiceNumber", "Номер накладной BDF");
            AddTranslation("invoiceNumber", "InvoiceNumber", "Номер счет-фактуры");
            AddTranslation("numberOfArticles", "NumberOfArticles", "Кол-во арт.");
            AddTranslation("theNumberOfBoxes", "TheNumberOfBoxes", "Кол-во коробок");
            AddTranslation("preliminaryNumberOfPallets", "PreliminaryNumberOfPallets", "Предварительное кол-во паллет");
            AddTranslation("actualNumberOfPallets", "ActualNumberOfPallets", "Фактическое кол-во паллет");
            AddTranslation("confirmedBoxes", "ConfirmedBoxes", "Подтвержденное кол-во коробок");
            AddTranslation("confirmedNumberOfPallets", "ConfirmedNumberOfPallets", "Подтвержденное кол-во паллет");
            AddTranslation("weightKg", "WeightKg", "Вес, кг");
            AddTranslation("orderAmountExcludingVAT", "OrderAmountExcludingVAT", "Сумма заказа, без НДС");
            AddTranslation("tTNAmountExcludingVAT", "TTNAmountExcludingVAT", "Сумма по ТТН, без НДС");
            AddTranslation("region", "Region", "Регион");
            AddTranslation("city", "City", "Город");
            AddTranslation("shippingAddress", "ShippingAddress", "Адрес отгрузки");
            AddTranslation("deliveryAddress", "DeliveryAddress", "Адрес доставки");
            AddTranslation("customerAvizTime", "CustomerAvizTime", "Время авизации у клиента");
            AddTranslation("orderComments", "OrderComments", "Комментарии по заказу");
            AddTranslation("typeOfEquipment", "TypeOfEquipment", "Тип комплектации");
            AddTranslation("plannedArrivalTimeSlotBDFWarehouse", "PlannedArrivalTimeSlotBDFWarehouse", "Плановое прибытие/тайм-слот (склад БДФ)");
            AddTranslation("arrivalTimeForLoadingBDFWarehouse", "ArrivalTimeForLoadingBDFWarehouse", "Время прибытия на загрузку  (склад БДФ)");
            AddTranslation("departureTimeFromTheBDFWarehouse", "DepartureTimeFromTheBDFWarehouse", "Время убытия со склада БДФ");
            AddTranslation("actualDateOfArrivalAtTheConsignee", "ActualDateOfArrivalAtTheConsignee", "Фактическая дата прибытия к грузополучателю");
            AddTranslation("arrivalTimeToConsignee", "ArrivalTimeToConsignee", "Время прибытия к грузополучателю");
            AddTranslation("dateOfDepartureFromTheConsignee", "DateOfDepartureFromTheConsignee", "Дата убытия от грузополучателя");
            AddTranslation("departureTimeFromConsignee", "DepartureTimeFromConsignee", "Время убытия от грузополучателя");
            AddTranslation("theNumberOfHoursOfDowntime", "TheNumberOfHoursOfDowntime", "Кол-во часов простоя машин");
            AddTranslation("returnInformation", "ReturnInformation", "Информация по возвратам");
            AddTranslation("returnShippingAccountNo", "ReturnShippingAccountNo", "№ счета за перевозку возврата");
            AddTranslation("plannedReturnDate", "PlannedReturnDate", "Плановый срок возврата");
            AddTranslation("actualReturnDate", "ActualReturnDate", "Фактический срок возврата");
            AddTranslation("majorAdoptionNumber", "MajorAdoptionNumber", "Номер приемного акта Мейджор");
            AddTranslation("avization", "Avization", "Авизация");
            AddTranslation("orderItems", "OrderItems", "Позиции в заказе");
            AddTranslation("orderCreationDate", "OrderCreationDate", "Дата создания заказа");
            AddTranslation("shipping", "Shipping", "Перевозка");
            AddTranslation("shippings", "Shippings", "Перевозка");
            AddTranslation("transportationNumber", "TransportationNumber", "Номер перевозки");
            AddTranslation("deliveryMethod", "DeliveryMethod", "Способ доставки");
            AddTranslation("thermalMode", "ThermalMode", "Терморежим");
            AddTranslation("billingMethod", "BillingMethod", "Способ тарификации");
            AddTranslation("transportCompany", "TransportCompany", "Транспортная компания");
            AddTranslation("deliveryInvoiceNumber", "DeliveryInvoiceNumber", "Номер счета за доставку");
            AddTranslation("commentsReasonsForDeviationFromTheSchedule", "CommentsReasonsForDeviationFromTheSchedule", "Комментарии (причины отклонения от графика)");
            AddTranslation("transportationCostWithoutVAT", "TransportationCostWithoutVAT", "Стоимость перевозки, без НДС");
            AddTranslation("returnShippingCostExcludingVAT", "ReturnShippingCostExcludingVAT", "Стоимость перевозки возврата, без НДС");
            AddTranslation("additionalShippingCostsExcludingVAT", "AdditionalShippingCostsExcludingVAT", "Дополнительные расходы на доставку, без НДС");
            AddTranslation("additionalShippingCostsComments", "AdditionalShippingCostsComments", "Дополнительные расходы на доставку (комментарии)");
            AddTranslation("waybill", "Waybill", "Транспортная накладная");
            AddTranslation("waybillTorg12", "WaybillTorg12", "Товарная накладная(Торг-12)");
            AddTranslation("waybillTransportSection", "WaybillTransportSection", "Товарно-Транспортная накладная +Транспортный раздел");
            AddTranslation("invoice", "Invoice", "Счет-фактура");
            AddTranslation("actualReturnDate", "ActualReturnDate", "Фактическая дата возврата документов");
            AddTranslation("deliveryStatus", "DeliveryStatus", "Статус доставки");
            AddTranslation("amountConfirmedByShipper", "AmountConfirmedByShipper", "Сумма подтверждена грузоотправителем");
            AddTranslation("amountConfirmedByTC", "AmountConfirmedByTC", "Сумма подтверждена ТК");
            AddTranslation("tariff", "Tariff", "Тариф");
            AddTranslation("tariffs", "Tariffs", "Тарифы");
            AddTranslation("cityOfShipment", "CityOfShipment", "Город отгрузки");
            AddTranslation("deliveryCity", "DeliveryCity", "Город доставки");
            AddTranslation("vehicleType", "VehicleType", "Тип ТС");
            AddTranslation("fTLBet", "FTLBet", "Ставка FTL");
            AddTranslation("lTLRate1", "LTLRate1", "Ставка LTL 1");
            AddTranslation("lTLRate2", "LTLRate2", "Ставка LTL 2");
            AddTranslation("betLTL3", "BetLTL3", "Ставка LTL 3");
            AddTranslation("lTLRate4", "LTLRate4", "Ставка LTL 4");
            AddTranslation("lTLRate5", "LTLRate5", "Ставка LTL 5");
            AddTranslation("lTLRate6", "LTLRate6", "Ставка LTL 6");
            AddTranslation("lTLRate7", "LTLRate7", "Ставка LTL 7");
            AddTranslation("lTLBet8", "LTLBet8", "Ставка LTL 8");
            AddTranslation("lTLRate9", "LTLRate9", "Ставка LTL 9");
            AddTranslation("lTLRate10", "LTLRate10", "Ставка LTL 10");
            AddTranslation("lTLRate11", "LTLRate11", "Ставка LTL 11");
            AddTranslation("lTLRate12", "LTLRate12", "Ставка LTL 12");
            AddTranslation("lTLRate13", "LTLRate13", "Ставка LTL 13");
            AddTranslation("lTLRate14", "LTLRate14", "Ставка LTL 14");
            AddTranslation("betLTL15", "BetLTL15", "Ставка LTL 15");
            AddTranslation("lTLRate16", "LTLRate16", "Ставка LTL 16");
            AddTranslation("betLTL17", "BetLTL17", "Ставка LTL 17");
            AddTranslation("betLTL18", "BetLTL18", "Ставка LTL 18");
            AddTranslation("lTLRate19", "LTLRate19", "Ставка LTL 19");
            AddTranslation("lTLRate20", "LTLRate20", "Ставка LTL 20");
            AddTranslation("lTLRate21", "LTLRate21", "Ставка LTL 21");
            AddTranslation("lTLRate22", "LTLRate22", "Ставка LTL 22");
            AddTranslation("lTLRate23", "LTLRate23", "Ставка LTL 23");
            AddTranslation("lTLRate24", "LTLRate24", "Ставка LTL 24");
            AddTranslation("lTLRate25", "LTLRate25", "Ставка LTL 25");
            AddTranslation("lTLBet26", "LTLBet26", "Ставка LTL 26");
            AddTranslation("lTLRate27", "LTLRate27", "Ставка LTL 27");
            AddTranslation("lTLBet28", "LTLBet28", "Ставка LTL 28");
            AddTranslation("lTLRate29", "LTLRate29", "Ставка LTL 29");
            AddTranslation("lTLRate30", "LTLRate30", "Ставка LTL 30");
            AddTranslation("lTLRate31", "LTLRate31", "Ставка LTL 31");
            AddTranslation("betLTL32", "BetLTL32", "Ставка LTL 32");
            AddTranslation("lTLBid33", "LTLBid33", "Ставка LTL 33");
            AddTranslation("warehouse", "Warehouse", "Склад");
            AddTranslation("warehouses", "Warehouses", "Склады");
            AddTranslation("theNameOfTheWarehouse", "TheNameOfTheWarehouse", "Наименование склада");
            AddTranslation("soldToNumber", "SoldToNumber", "SoldTo number");
            AddTranslation("address", "Address", "Адрес");
            AddTranslation("leadtimeDays", "LeadtimeDays", "Leadtime, дней");
            AddTranslation("customerWarehouse", "CustomerWarehouse", "Склад клиента");
            AddTranslation("article", "Article", "Артикул");
            AddTranslation("articles", "Articles", "Артикулы");
            AddTranslation("sPGR", "SPGR", "SPGR");
            AddTranslation("description", "Description", "Описание");
            AddTranslation("nart", "Nart", "NART");
            AddTranslation("countryOfOrigin", "CountryOfOrigin", "Страна происхождения");
            AddTranslation("shelfLife", "ShelfLife", "Срок годности");
            AddTranslation("ean", "Ean", "EAN");
            AddTranslation("unitLengthGoodsMm", "UnitLengthGoodsMm", "Длина ед. товара, мм");
            AddTranslation("widthUnitsGoodsMm", "WidthUnitsGoodsMm", "Ширина ед. товара, мм");
            AddTranslation("unitHeightGoodsMm", "UnitHeightGoodsMm", "Высота ед. товара, мм");
            AddTranslation("weightUnitsGrossProductG", "WeightUnitsGrossProductG", "Вес ед. товара брутто, г");
            AddTranslation("weightUnitsNetGoodsG", "WeightUnitsNetGoodsG", "Вес ед. товара нетто, г");
            AddTranslation("eANShrink", "EANShrink", "EAN, shrink");
            AddTranslation("piecesInShrink", "PiecesInShrink", "Штук в shrink");
            AddTranslation("lengthShrinkMm", "LengthShrinkMm", "Длина shrink, мм");
            AddTranslation("widthShrinkMm", "WidthShrinkMm", "Ширина shrink, мм");
            AddTranslation("heightShrinkMm", "HeightShrinkMm", "Высота shrink, мм");
            AddTranslation("grossShrinkWeightG", "GrossShrinkWeightG", "Вес shrink брутто, г");
            AddTranslation("netWeightShrinkG", "NetWeightShrinkG", "Вес shrink нетто, г");
            AddTranslation("eANBox", "EANBox", "EAN, короб");
            AddTranslation("piecesInABox", "PiecesInABox", "Штук в коробе");
            AddTranslation("boxLengthMm", "BoxLengthMm", "Длина короба, мм");
            AddTranslation("widthOfABoxMm", "WidthOfABoxMm", "Ширина короба, мм");
            AddTranslation("boxHeightMm", "BoxHeightMm", "Высота короба, мм");
            AddTranslation("grossBoxWeightG", "GrossBoxWeightG", "Вес короба брутто, г");
            AddTranslation("netBoxWeightG", "NetBoxWeightG", "Вес короба нетто, г");
            AddTranslation("piecesInALayer", "PiecesInALayer", "Штук в слое");
            AddTranslation("layerLengthMm", "LayerLengthMm", "Длина слоя, мм");
            AddTranslation("layerWidthMm", "LayerWidthMm", "Ширина слоя, мм");
            AddTranslation("layerHeightMm", "LayerHeightMm", "Высота слоя, мм");
            AddTranslation("grossLayerWeightMm", "GrossLayerWeightMm", "Вес слоя брутто, мм");
            AddTranslation("netWeightMm", "NetWeightMm", "Вес слоя нетто, мм");
            AddTranslation("eANPallet", "EANPallet", "EAN, паллета");
            AddTranslation("piecesOnAPallet", "PiecesOnAPallet", "Штук на паллете");
            AddTranslation("palletLengthMm", "PalletLengthMm", "Длина паллеты, мм");
            AddTranslation("widthOfPalletsMm", "WidthOfPalletsMm", "Ширина паллеты, мм");
            AddTranslation("palletHeightMm", "PalletHeightMm", "Высота паллеты, мм");
            AddTranslation("grossPalletWeightG", "GrossPalletWeightG", "Вес паллеты брутто, г");
            AddTranslation("netWeightPalletsG", "NetWeightPalletsG", "Вес паллеты нетто, г");
            AddTranslation("transportCompanies", "TransportCompanies", "Транспортные компании");
            AddTranslation("title", "Title", "Название");
            AddTranslation("contractNumber", "ContractNumber", "Номер договора");
            AddTranslation("dateOfPowerOfAttorney", "DateOfPowerOfAttorney", "Дата доверенности");
            /*end of add translates*/
            
        }

        private void AddTransportation(string from, string to)
        {
            Database.Insert("Transportations", new string[] {"Id", "From", "To"},
                    new string[] {(Guid.NewGuid()).ToString(), from, to});
        }

        private void AddOrder(string incoming)
        {
            Database.Insert("Orders", new string[] {"Id", "Incoming"},
                    new string[] {(Guid.NewGuid()).ToString(), incoming});
        }

        private string AddTranslation(string name, string en, string ru)
        {
            var id = (Guid.NewGuid()).ToString();
            Database.Insert("Translations", new string[] {"Id", "Name", "En", "Ru"},
                new string[] {id, name, en, ru});
            return id;
        }

        private string AddRole(string name)
        {
            var id = (Guid.NewGuid()).ToString();
            Database.Insert("Roles", new string[] {"Id", "Name"},
                new string[] {id, name});
            return id;
        }

        private void AddUser(string name, string roleid, string email, string passwordhash)
        {
            Database.Insert("Users", new string[] {"Id", "Name", "RoleId", "Email", "IsActive", "FieldsConfig", "PasswordHash"},
                new string[] {(Guid.NewGuid()).ToString(), name, roleid, email, "true", "", passwordhash});
        }
    }
}