using System;
using System.Data;
using Infrastructure.Extensions;
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
                new Column("Status", DbType.String),
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
                new Column("LTLRate17", DbType.String),
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

            Database.AddTable("TransportСompanies",
                new Column("Title", DbType.String),
                new Column("ContractNumber", DbType.String),
                new Column("DateOfPowerOfAttorney", DbType.String),
                /*general fields for TransportСompanies*/
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey)
            );
            Database.AddIndex("transportСompanies_pk", true, "TransportСompanies", "Id");

            /*end of add tables*/
            
            AddTranslation("UserNotFound", "User not found", "Пользователь не найден или не активен");
            AddTranslation("UserIncorrectData", "The username or password you entered is incorrect", "Неверное имя пользователя или пароль");
            AddTranslation("Users", "Users", "Пользователи");
            AddTranslation("Roles", "Roles", "Роли");
            AddTranslation("IsActive", "IsActive", "Активен");

            AddTranslation("login_btn", "Login", "Войти");
            AddTranslation("password", "password", "Пароль");
            AddTranslation("login", "email", "email");
            AddTranslation("dictionaries", "Dictionaries", "Справочники");
            AddTranslation("users", "Users", "Пользователи");
            AddTranslation("roles", "Roles", "Роли");
            AddTranslation("create_btn", "Create", "Создать");
            AddTranslation("search_all_fields", "Search all fields", "Искать по всем полям");
            AddTranslation("reset_filters", "Reset filters", "Сбросить фильтры");

            /*start of add translates*/
            AddTranslation("login_welcome", "login_welcome", "TMS Beiersdorf");
            AddTranslation("login_support", "login_support", "TMS для компании Beiersdorf");
            AddTranslation("Order", "Order", "Заказ");
            AddTranslation("Orders", "Orders", "Заказы");
            AddTranslation("SalesOrderNumber", "SalesOrderNumber", "Номер заказ клиента");
            AddTranslation("OrderDate", "OrderDate", "Дата заказа");
            AddTranslation("TypeOfOrder", "TypeOfOrder", "Тип заказа");
            AddTranslation("Payer", "Payer", "Плательщик");
            AddTranslation("CustomerName", "CustomerName", "Название клиента");
            AddTranslation("SoldTo", "SoldTo", "Sold-to");
            AddTranslation("ShippingDate", "ShippingDate", "Дата отгрузки");
            AddTranslation("DaysOnTheRoad", "DaysOnTheRoad", "Дней в пути");
            AddTranslation("DeliveryDate", "DeliveryDate", "Дата доставки");
            AddTranslation("BDFInvoiceNumber", "BDFInvoiceNumber", "Номер накладной BDF");
            AddTranslation("InvoiceNumber", "InvoiceNumber", "Номер счет-фактуры");
            AddTranslation("NumberOfArticles", "NumberOfArticles", "Кол-во арт.");
            AddTranslation("TheNumberOfBoxes", "TheNumberOfBoxes", "Кол-во коробок");
            AddTranslation("PreliminaryNumberOfPallets", "PreliminaryNumberOfPallets", "Предварительное кол-во паллет");
            AddTranslation("ActualNumberOfPallets", "ActualNumberOfPallets", "Фактическое кол-во паллет");
            AddTranslation("ConfirmedBoxes", "ConfirmedBoxes", "Подтвержденное кол-во коробок");
            AddTranslation("ConfirmedNumberOfPallets", "ConfirmedNumberOfPallets", "Подтвержденное кол-во паллет");
            AddTranslation("WeightKg", "WeightKg", "Вес, кг");
            AddTranslation("OrderAmountExcludingVAT", "OrderAmountExcludingVAT", "Сумма заказа, без НДС");
            AddTranslation("TTNAmountExcludingVAT", "TTNAmountExcludingVAT", "Сумма по ТТН, без НДС");
            AddTranslation("Region", "Region", "Регион");
            AddTranslation("City", "City", "Город");
            AddTranslation("ShippingAddress", "ShippingAddress", "Адрес отгрузки");
            AddTranslation("DeliveryAddress", "DeliveryAddress", "Адрес доставки");
            AddTranslation("CustomerAvizTime", "CustomerAvizTime", "Время авизации у клиента");
            AddTranslation("OrderComments", "OrderComments", "Комментарии по заказу");
            AddTranslation("TypeOfEquipment", "TypeOfEquipment", "Тип комплектации");
            AddTranslation("PlannedArrivalTimeSlotBDFWarehouse", "PlannedArrivalTimeSlotBDFWarehouse", "Плановое прибытие/тайм-слот (склад БДФ)");
            AddTranslation("ArrivalTimeForLoadingBDFWarehouse", "ArrivalTimeForLoadingBDFWarehouse", "Время прибытия на загрузку  (склад БДФ)");
            AddTranslation("DepartureTimeFromTheBDFWarehouse", "DepartureTimeFromTheBDFWarehouse", "Время убытия со склада БДФ");
            AddTranslation("ActualDateOfArrivalAtTheConsignee", "ActualDateOfArrivalAtTheConsignee", "Фактическая дата прибытия к грузополучателю");
            AddTranslation("ArrivalTimeToConsignee", "ArrivalTimeToConsignee", "Время прибытия к грузополучателю");
            AddTranslation("DateOfDepartureFromTheConsignee", "DateOfDepartureFromTheConsignee", "Дата убытия от грузополучателя");
            AddTranslation("DepartureTimeFromConsignee", "DepartureTimeFromConsignee", "Время убытия от грузополучателя");
            AddTranslation("TheNumberOfHoursOfDowntime", "TheNumberOfHoursOfDowntime", "Кол-во часов простоя машин");
            AddTranslation("ReturnInformation", "ReturnInformation", "Информация по возвратам");
            AddTranslation("ReturnShippingAccountNo", "ReturnShippingAccountNo", "№ счета за перевозку возврата");
            AddTranslation("PlannedReturnDate", "PlannedReturnDate", "Плановый срок возврата");
            AddTranslation("ActualReturnDate", "ActualReturnDate", "Фактический срок возврата");
            AddTranslation("MajorAdoptionNumber", "MajorAdoptionNumber", "Номер приемного акта Мейджор");
            AddTranslation("Avization", "Avization", "Авизация");
            AddTranslation("Status", "Status", "Статус");
            AddTranslation("OrderItems", "OrderItems", "Позиции в заказе");
            AddTranslation("OrderCreationDate", "OrderCreationDate", "Дата создания заказа");
            AddTranslation("Shipping", "Shipping", "Перевозка");
            AddTranslation("Shippings", "Shippings", "Перевозка");
            AddTranslation("TransportationNumber", "TransportationNumber", "Номер перевозки");
            AddTranslation("DeliveryMethod", "DeliveryMethod", "Способ доставки");
            AddTranslation("ThermalMode", "ThermalMode", "Терморежим");
            AddTranslation("BillingMethod", "BillingMethod", "Способ тарификации");
            AddTranslation("TransportCompany", "TransportCompany", "Транспортная компания");
            AddTranslation("DeliveryInvoiceNumber", "DeliveryInvoiceNumber", "Номер счета за доставку");
            AddTranslation("CommentsReasonsForDeviationFromTheSchedule", "CommentsReasonsForDeviationFromTheSchedule", "Комментарии (причины отклонения от графика)");
            AddTranslation("TransportationCostWithoutVAT", "TransportationCostWithoutVAT", "Стоимость перевозки, без НДС");
            AddTranslation("ReturnShippingCostExcludingVAT", "ReturnShippingCostExcludingVAT", "Стоимость перевозки возврата, без НДС");
            AddTranslation("AdditionalShippingCostsExcludingVAT", "AdditionalShippingCostsExcludingVAT", "Дополнительные расходы на доставку, без НДС");
            AddTranslation("AdditionalShippingCostsComments", "AdditionalShippingCostsComments", "Дополнительные расходы на доставку (комментарии)");
            AddTranslation("Waybill", "Waybill", "Транспортная накладная");
            AddTranslation("WaybillTorg12", "WaybillTorg12", "Товарная накладная(Торг-12)");
            AddTranslation("WaybillTransportSection", "WaybillTransportSection", "Товарно-Транспортная накладная +Транспортный раздел");
            AddTranslation("Invoice", "Invoice", "Счет-фактура");
            AddTranslation("ActualReturnDate", "ActualReturnDate", "Фактическая дата возврата документов");
            AddTranslation("DeliveryStatus", "DeliveryStatus", "Статус доставки");
            AddTranslation("AmountConfirmedByShipper", "AmountConfirmedByShipper", "Сумма подтверждена грузоотправителем");
            AddTranslation("AmountConfirmedByTC", "AmountConfirmedByTC", "Сумма подтверждена ТК");
            AddTranslation("Tariff", "Tariff", "Тариф");
            AddTranslation("Tariffs", "Tariffs", "Тарифы");
            AddTranslation("CityOfShipment", "CityOfShipment", "Город отгрузки");
            AddTranslation("DeliveryCity", "DeliveryCity", "Город доставки");
            AddTranslation("VehicleType", "VehicleType", "Тип ТС");
            AddTranslation("FTLBet", "FTLBet", "Ставка FTL");
            AddTranslation("LTLRate1", "LTLRate1", "Ставка LTL 1");
            AddTranslation("LTLRate2", "LTLRate2", "Ставка LTL 2");
            AddTranslation("BetLTL3", "BetLTL3", "Ставка LTL 3");
            AddTranslation("LTLRate4", "LTLRate4", "Ставка LTL 4");
            AddTranslation("LTLRate5", "LTLRate5", "Ставка LTL 5");
            AddTranslation("LTLRate6", "LTLRate6", "Ставка LTL 6");
            AddTranslation("LTLRate7", "LTLRate7", "Ставка LTL 7");
            AddTranslation("LTLBet8", "LTLBet8", "Ставка LTL 8");
            AddTranslation("LTLRate9", "LTLRate9", "Ставка LTL 9");
            AddTranslation("LTLRate10", "LTLRate10", "Ставка LTL 10");
            AddTranslation("LTLRate11", "LTLRate11", "Ставка LTL 11");
            AddTranslation("LTLRate12", "LTLRate12", "Ставка LTL 12");
            AddTranslation("LTLRate13", "LTLRate13", "Ставка LTL 13");
            AddTranslation("LTLRate14", "LTLRate14", "Ставка LTL 14");
            AddTranslation("BetLTL15", "BetLTL15", "Ставка LTL 15");
            AddTranslation("LTLRate16", "LTLRate16", "Ставка LTL 16");
            AddTranslation("LTLRate17", "LTLRate17", "Ставка LTL 17");
            AddTranslation("BetLTL18", "BetLTL18", "Ставка LTL 18");
            AddTranslation("LTLRate19", "LTLRate19", "Ставка LTL 19");
            AddTranslation("LTLRate20", "LTLRate20", "Ставка LTL 20");
            AddTranslation("LTLRate21", "LTLRate21", "Ставка LTL 21");
            AddTranslation("LTLRate22", "LTLRate22", "Ставка LTL 22");
            AddTranslation("LTLRate23", "LTLRate23", "Ставка LTL 23");
            AddTranslation("LTLRate24", "LTLRate24", "Ставка LTL 24");
            AddTranslation("LTLRate25", "LTLRate25", "Ставка LTL 25");
            AddTranslation("LTLBet26", "LTLBet26", "Ставка LTL 26");
            AddTranslation("LTLRate27", "LTLRate27", "Ставка LTL 27");
            AddTranslation("LTLBet28", "LTLBet28", "Ставка LTL 28");
            AddTranslation("LTLRate29", "LTLRate29", "Ставка LTL 29");
            AddTranslation("LTLRate30", "LTLRate30", "Ставка LTL 30");
            AddTranslation("LTLRate31", "LTLRate31", "Ставка LTL 31");
            AddTranslation("BetLTL32", "BetLTL32", "Ставка LTL 32");
            AddTranslation("LTLBid33", "LTLBid33", "Ставка LTL 33");
            AddTranslation("Warehouse", "Warehouse", "Склад");
            AddTranslation("Warehouses", "Warehouses", "Склады");
            AddTranslation("TheNameOfTheWarehouse", "TheNameOfTheWarehouse", "Наименование склада");
            AddTranslation("SoldToNumber", "SoldToNumber", "SoldTo number");
            AddTranslation("Address", "Address", "Адрес");
            AddTranslation("LeadtimeDays", "LeadtimeDays", "Leadtime, дней");
            AddTranslation("CustomerWarehouse", "CustomerWarehouse", "Склад клиента");
            AddTranslation("Article", "Article", "Артикул");
            AddTranslation("Articles", "Articles", "Артикулы");
            AddTranslation("SPGR", "SPGR", "SPGR");
            AddTranslation("Description", "Description", "Описание");
            AddTranslation("Nart", "Nart", "NART");
            AddTranslation("CountryOfOrigin", "CountryOfOrigin", "Страна происхождения");
            AddTranslation("ShelfLife", "ShelfLife", "Срок годности");
            AddTranslation("Ean", "Ean", "EAN");
            AddTranslation("UnitLengthGoodsMm", "UnitLengthGoodsMm", "Длина ед. товара, мм");
            AddTranslation("WidthUnitsGoodsMm", "WidthUnitsGoodsMm", "Ширина ед. товара, мм");
            AddTranslation("UnitHeightGoodsMm", "UnitHeightGoodsMm", "Высота ед. товара, мм");
            AddTranslation("WeightUnitsGrossProductG", "WeightUnitsGrossProductG", "Вес ед. товара брутто, г");
            AddTranslation("WeightUnitsNetGoodsG", "WeightUnitsNetGoodsG", "Вес ед. товара нетто, г");
            AddTranslation("EANShrink", "EANShrink", "EAN, shrink");
            AddTranslation("PiecesInShrink", "PiecesInShrink", "Штук в shrink");
            AddTranslation("LengthShrinkMm", "LengthShrinkMm", "Длина shrink, мм");
            AddTranslation("WidthShrinkMm", "WidthShrinkMm", "Ширина shrink, мм");
            AddTranslation("HeightShrinkMm", "HeightShrinkMm", "Высота shrink, мм");
            AddTranslation("GrossShrinkWeightG", "GrossShrinkWeightG", "Вес shrink брутто, г");
            AddTranslation("NetWeightShrinkG", "NetWeightShrinkG", "Вес shrink нетто, г");
            AddTranslation("EANBox", "EANBox", "EAN, короб");
            AddTranslation("PiecesInABox", "PiecesInABox", "Штук в коробе");
            AddTranslation("BoxLengthMm", "BoxLengthMm", "Длина короба, мм");
            AddTranslation("WidthOfABoxMm", "WidthOfABoxMm", "Ширина короба, мм");
            AddTranslation("BoxHeightMm", "BoxHeightMm", "Высота короба, мм");
            AddTranslation("GrossBoxWeightG", "GrossBoxWeightG", "Вес короба брутто, г");
            AddTranslation("NetBoxWeightG", "NetBoxWeightG", "Вес короба нетто, г");
            AddTranslation("PiecesInALayer", "PiecesInALayer", "Штук в слое");
            AddTranslation("LayerLengthMm", "LayerLengthMm", "Длина слоя, мм");
            AddTranslation("LayerWidthMm", "LayerWidthMm", "Ширина слоя, мм");
            AddTranslation("LayerHeightMm", "LayerHeightMm", "Высота слоя, мм");
            AddTranslation("GrossLayerWeightMm", "GrossLayerWeightMm", "Вес слоя брутто, мм");
            AddTranslation("NetWeightMm", "NetWeightMm", "Вес слоя нетто, мм");
            AddTranslation("EANPallet", "EANPallet", "EAN, паллета");
            AddTranslation("PiecesOnAPallet", "PiecesOnAPallet", "Штук на паллете");
            AddTranslation("PalletLengthMm", "PalletLengthMm", "Длина паллеты, мм");
            AddTranslation("WidthOfPalletsMm", "WidthOfPalletsMm", "Ширина паллеты, мм");
            AddTranslation("PalletHeightMm", "PalletHeightMm", "Высота паллеты, мм");
            AddTranslation("GrossPalletWeightG", "GrossPalletWeightG", "Вес паллеты брутто, г");
            AddTranslation("NetWeightPalletsG", "NetWeightPalletsG", "Вес паллеты нетто, г");
            AddTranslation("TransportСompany", "TransportСompany", "Транспортная компания");
            AddTranslation("TransportСompanies", "TransportСompanies", "Транспортные компании");
            AddTranslation("Title", "Title", "Название");
            AddTranslation("ContractNumber", "ContractNumber", "Номер договора");
            AddTranslation("DateOfPowerOfAttorney", "DateOfPowerOfAttorney", "Дата доверенности");
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