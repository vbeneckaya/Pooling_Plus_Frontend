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
            var customerServiceRoleId = AddRole("CustomerService");
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
            AddUser("Андрей Городецкий", customerServiceRoleId, "andry@tms.ru", "123".GetHash());
            AddUser("Максим Координатович", transportСoordinatorId, "max@tms.ru", "123".GetHash());
            AddUser("Сергей Газельев", transportCompanyEmployee, "gazelev@tms.ru", "123".GetHash());

            Database.AddTable("Translations",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String.WithSize(100)),
                new Column("En", DbType.String.WithSize(100)),
                new Column("Ru", DbType.String.WithSize(100))
                );
            Database.AddIndex("translations_pk", true, "Translations", "Id");

            Database.AddTable("Injections",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Type", DbType.String.WithSize(100)),
                new Column("FileName", DbType.String.WithSize(200)),
                new Column("Status", DbType.String.WithSize(10)),
                new Column("ProcessTimeUtc", DbType.DateTime));
            Database.AddIndex("injections_pk", true, "Injections", "Id");

            Database.AddTable("TaskProperties",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("TaskName", DbType.String.WithSize(100)),
                new Column("Properties", DbType.String.WithSize(1000)));
            Database.AddIndex("TaskProperties_pk", true, "TaskProperties", "Id");

            AddTaskProperties("ImportProducts", "ConnectionString=sftp://bsdf-usr:e7%24xFSMgYw%2Bc4N@213.189.208.101/;Folder=/Test/OUT;ViewHours=168");
            AddTaskProperties("ImportOrder", "ConnectionString=sftp://bsdf-usr:e7%24xFSMgYw%2Bc4N@213.189.208.101/;Folder=/Test/OUT;ViewHours=168");

            /*start of add tables*/
            Database.AddTable("Orders",
                new Column("Status", DbType.Int32, ColumnProperty.Null),
                new Column("OrderNumber", DbType.String),
                new Column("OrderDate", DbType.DateTime, ColumnProperty.Null),
                new Column("OrderType", DbType.Int32, ColumnProperty.Null),
                new Column("Payer", DbType.String),
                new Column("ClientName", DbType.String),
                new Column("SoldTo", DbType.String),
                new Column("TemperatureMin", DbType.Int32, ColumnProperty.Null),
                new Column("TemperatureMax", DbType.Int32, ColumnProperty.Null),
                new Column("ShippingDate", DbType.DateTime, ColumnProperty.Null),
                new Column("TransitDays", DbType.Int32, ColumnProperty.Null),
                new Column("DeliveryDate", DbType.DateTime, ColumnProperty.Null),
                new Column("BDFInvoiceNumber", DbType.String),
                new Column("InvoiceNumber", DbType.String),
                new Column("ArticlesCount", DbType.Int32, ColumnProperty.Null),
                new Column("BoxesCount", DbType.Int32, ColumnProperty.Null),
                new Column("ConfirmedBoxesCount", DbType.Int32, ColumnProperty.Null),
                new Column("PalletsCount", DbType.Int32, ColumnProperty.Null),
                new Column("ConfirmedPalletsCount", DbType.Int32, ColumnProperty.Null),
                new Column("ActualPalletsCount", DbType.Int32, ColumnProperty.Null),
                new Column("WeightKg", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("ActualWeightKg", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("OrderAmountExcludingVAT", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("InvoiceAmountExcludingVAT", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("DeliveryRegion", DbType.String),
                new Column("DeliveryCity", DbType.String),
                new Column("ShippingAddress", DbType.String),
                new Column("DeliveryAddress", DbType.String),
                new Column("ShippingStatus", DbType.Int32, ColumnProperty.Null),
                new Column("DeliveryStatus", DbType.Int32, ColumnProperty.Null),
                new Column("ClientAvisationTime", DbType.Time, ColumnProperty.Null),
                new Column("OrderComments", DbType.String),
                new Column("PickingTypeId", DbType.Guid, ColumnProperty.Null),
                new Column("PlannedArrivalTimeSlotBDFWarehouse", DbType.String),
                new Column("LoadingArrivalTime", DbType.DateTime, ColumnProperty.Null),
                new Column("LoadingDepartureTime", DbType.DateTime, ColumnProperty.Null),
                new Column("UnloadingArrivalTime", DbType.Date, ColumnProperty.Null),
                new Column("UnloadingDepartureTime", DbType.Date, ColumnProperty.Null),
                new Column("TrucksDowntime", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("ReturnInformation", DbType.String),
                new Column("ReturnShippingAccountNo", DbType.String),
                new Column("PlannedReturnDate", DbType.DateTime, ColumnProperty.Null),
                new Column("ActualReturnDate", DbType.DateTime, ColumnProperty.Null),
                new Column("MajorAdoptionNumber", DbType.String),
                new Column("OrderCreationDate", DbType.DateTime, ColumnProperty.Null),
                new Column("ShippingId", DbType.Guid, ColumnProperty.Null),
                new Column("ShippingWarehouseId", DbType.Guid, ColumnProperty.Null),
                new Column("DeliveryWarehouseId", DbType.Guid, ColumnProperty.Null),
                /*general fields for Orders*/
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey)
            );
            Database.AddIndex("orders_pk", true, "Orders", "Id");

            Database.AddTable("OrderItems",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("OrderId", DbType.Guid),
                new Column("Nart", DbType.String),
                new Column("Quantity", DbType.Int32)
            );
            Database.AddIndex("OrderItems_pk", true, "OrderItems", "Id");
            Database.AddIndex("OrderItems_order_fk", false, "OrderItems", "OrderId");

            Database.AddTable("Shippings",
                new Column("ShippingNumber", DbType.String),
                new Column("DeliveryType", DbType.Int32, ColumnProperty.Null),
                new Column("TemperatureMin", DbType.Int32, ColumnProperty.Null),
                new Column("TemperatureMax", DbType.Int32, ColumnProperty.Null),
                new Column("TarifficationType", DbType.Int32, ColumnProperty.Null),
                new Column("CarrierId", DbType.Guid, ColumnProperty.Null),
                new Column("VehicleTypeId", DbType.Guid, ColumnProperty.Null),
                new Column("PalletsCount", DbType.Int32, ColumnProperty.Null),
                new Column("ManualPalletsCount", DbType.Boolean, defaultValue: false),
                new Column("ActualPalletsCount", DbType.Int32, ColumnProperty.Null),
                new Column("ManualActualPalletsCount", DbType.Boolean, defaultValue: false),
                new Column("ConfirmedPalletsCount", DbType.Int32, ColumnProperty.Null),
                new Column("ManualConfirmedPalletsCount", DbType.Boolean, defaultValue: false),
                new Column("WeightKg", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("ManualWeightKg", DbType.Boolean, defaultValue: false),
                new Column("ActualWeightKg", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("ManualActualWeightKg", DbType.Boolean, defaultValue: false),
                new Column("PlannedArrivalTimeSlotBDFWarehouse", DbType.String),
                new Column("LoadingArrivalTime", DbType.DateTime, ColumnProperty.Null),
                new Column("LoadingDepartureTime", DbType.DateTime, ColumnProperty.Null),
                new Column("DeliveryInvoiceNumber", DbType.String),
                new Column("DeviationReasonsComments", DbType.String),
                new Column("TotalDeliveryCost", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("ManualTotalDeliveryCost", DbType.Boolean, defaultValue: false),
                new Column("OtherCosts", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("DeliveryCostWithoutVAT", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("ReturnCostWithoutVAT", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("InvoiceAmountWithoutVAT", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("AdditionalCostsWithoutVAT", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("AdditionalCostsComments", DbType.String),
                new Column("TrucksDowntime", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("ManualTrucksDowntime", DbType.Boolean, defaultValue: false),
                new Column("ReturnRate", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("AdditionalPointRate", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("DowntimeRate", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("BlankArrivalRate", DbType.Decimal.WithSize(19, 5), ColumnProperty.Null),
                new Column("BlankArrival", DbType.Boolean, defaultValue: false),
                new Column("Waybill", DbType.Boolean, defaultValue: false),
                new Column("WaybillTorg12", DbType.Boolean, defaultValue: false),
                new Column("TransportWaybill", DbType.Boolean, defaultValue: false),
                new Column("Invoice", DbType.Boolean, defaultValue: false),
                new Column("DocumentsReturnDate", DbType.DateTime, ColumnProperty.Null),
                new Column("ActualDocumentsReturnDate", DbType.DateTime, ColumnProperty.Null),
                new Column("InvoiceNumber", DbType.String),
                new Column("Status", DbType.Int64, ColumnProperty.Null),
                new Column("CostsConfirmedByShipper", DbType.Boolean, defaultValue: false),
                new Column("CostsConfirmedByCarrier", DbType.Boolean, defaultValue: false),
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
                new Column("WarehouseName", DbType.String),
                new Column("SoldToNumber", DbType.String),
                new Column("Region", DbType.String),
                new Column("City", DbType.String),
                new Column("Address", DbType.String),
                new Column("PickingTypeId", DbType.Guid, ColumnProperty.Null),
                new Column("LeadtimeDays", DbType.String),
                new Column("CustomerWarehouse", DbType.String),
                new Column("UsePickingType", DbType.String),
                /*general fields for Warehouses*/
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey)
            );
            Database.AddIndex("warehouses_pk", true, "Warehouses", "Id");

            Database.AddTable("Articles",
                new Column("SPGR", DbType.String, ColumnProperty.Null),
                new Column("Description", DbType.String, ColumnProperty.Null),
                new Column("Nart", DbType.String),
                new Column("CountryOfOrigin", DbType.String, ColumnProperty.Null),
                new Column("ShelfLife", DbType.Int32, ColumnProperty.Null),
                new Column("Status", DbType.String),
                new Column("Ean", DbType.String, ColumnProperty.Null),
                new Column("UnitLengthGoodsMm", DbType.Int32, ColumnProperty.Null),
                new Column("WidthUnitsGoodsMm", DbType.Int32, ColumnProperty.Null),
                new Column("UnitHeightGoodsMm", DbType.Int32, ColumnProperty.Null),
                new Column("WeightUnitsGrossProductG", DbType.Int32, ColumnProperty.Null),
                new Column("WeightUnitsNetGoodsG", DbType.Int32, ColumnProperty.Null),
                new Column("EANShrink", DbType.String, ColumnProperty.Null),
                new Column("PiecesInShrink", DbType.Int32, ColumnProperty.Null),
                new Column("LengthShrinkMm", DbType.Int32, ColumnProperty.Null),
                new Column("WidthShrinkMm", DbType.Int32, ColumnProperty.Null),
                new Column("HeightShrinkMm", DbType.Int32, ColumnProperty.Null),
                new Column("GrossShrinkWeightG", DbType.Int32, ColumnProperty.Null),
                new Column("NetWeightShrinkG", DbType.Int32, ColumnProperty.Null),
                new Column("EANBox", DbType.String, ColumnProperty.Null),
                new Column("PiecesInABox", DbType.Int32, ColumnProperty.Null),
                new Column("BoxLengthMm", DbType.Int32, ColumnProperty.Null),
                new Column("WidthOfABoxMm", DbType.Int32, ColumnProperty.Null),
                new Column("BoxHeightMm", DbType.Int32, ColumnProperty.Null),
                new Column("GrossBoxWeightG", DbType.Int32, ColumnProperty.Null),
                new Column("NetBoxWeightG", DbType.Int32, ColumnProperty.Null),
                new Column("PiecesInALayer", DbType.Int32, ColumnProperty.Null),
                new Column("LayerLengthMm", DbType.Int32, ColumnProperty.Null),
                new Column("LayerWidthMm", DbType.Int32, ColumnProperty.Null),
                new Column("LayerHeightMm", DbType.Int32, ColumnProperty.Null),
                new Column("GrossLayerWeightMm", DbType.Int32, ColumnProperty.Null),
                new Column("NetWeightMm", DbType.Int32, ColumnProperty.Null),
                new Column("EANPallet", DbType.String, ColumnProperty.Null),
                new Column("PiecesOnAPallet", DbType.Int32, ColumnProperty.Null),
                new Column("PalletLengthMm", DbType.Int32, ColumnProperty.Null),
                new Column("WidthOfPalletsMm", DbType.Int32, ColumnProperty.Null),
                new Column("PalletHeightMm", DbType.Int32, ColumnProperty.Null),
                new Column("GrossPalletWeightG", DbType.Int32, ColumnProperty.Null),
                new Column("NetWeightPalletsG", DbType.Int32, ColumnProperty.Null),
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

            Database.AddTable("VehicleTypes",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String, ColumnProperty.NotNull));
            Database.AddIndex("vehicleTypes_pk", true, "VehicleTypes", "Id");

            Database.AddTable("PickingTypes",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String, ColumnProperty.NotNull));
            Database.AddIndex("pickingTypes_pk", true, "PickingTypes", "Id");

            Database.AddTable("FileStorage",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("Data", DbType.Binary, ColumnProperty.NotNull));
            Database.AddIndex("fileStorage_pk", true, "FileStorage", "Id");

            Database.AddTable("DocumentTypes",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String, ColumnProperty.NotNull));
            Database.AddIndex("documentTypes_pk", true, "DocumentTypes", "Id");

            AddDocumentType("Накладная");
            AddDocumentType("Счет-фактура");
            AddDocumentType("Другое");

            Database.AddTable("Documents",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String, ColumnProperty.NotNull),
                new Column("PersistableId", DbType.Guid, ColumnProperty.NotNull),
                new Column("FileId", DbType.Guid, ColumnProperty.NotNull),
                new Column("TypeId", DbType.Guid, ColumnProperty.NotNull));
            Database.AddIndex("documents_pk", true, "Documents", "Id");
            Database.AddIndex("documents_persistableId_ix", false, "Documents", "PersistableId");
            Database.AddForeignKey("documents_fileStorage_fk", "Documents", "FileId", "FileStorage", "Id");
            Database.AddForeignKey("documents_documentTypes_fk", "Documents", "TypeId", "DocumentTypes", "Id");

            Database.AddTable("HistoryEntries",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("PersistableId", DbType.Guid, ColumnProperty.NotNull),
                new Column("UserId", DbType.Guid, ColumnProperty.Null),
                new Column("UserName", DbType.String),
                new Column("CreatedAt", DbType.DateTime),
                new Column("MessageKey", DbType.String),
                new Column("MessageArgs", DbType.String.WithSize(int.MaxValue)));
            Database.AddIndex("historyEntries_pk", true, "HistoryEntries", "Id");
            Database.AddIndex("historyEntries_persistableId_ix", false, "HistoryEntries", "PersistableId");
            Database.AddForeignKey("historyEntries_user_fk", "HistoryEntries", "UserId", "Users", "Id");
            /*end of add tables*/

            AddTranslation("UserNotFound", "User not found", "Пользователь не найден или не активен");
            AddTranslation("UserIncorrectData", "The username or password you entered is incorrect", "Неверное имя пользователя или пароль");
            AddTranslation("Users", "Users", "Пользователи");
            AddTranslation("Roles", "Roles", "Роли");
            AddTranslation("isActive", "Active", "Активен");
            AddTranslation("All", "All", "Все");
            AddTranslation("create_role_title", "Create role", "Создание роли");
            AddTranslation("create_role", "Create role", "Создать роль");
            AddTranslation("edit_role", "Edit role {{name}}", "Редактировать роль {{name}}");
            AddTranslation("create_user_title", "Create user", "Создание пользователя");
            AddTranslation("create_user", "Create user", "Создать пользователя");
            AddTranslation("edit_user", "Edit user {{name}}", "Редактирование пользователя {{name}}");

            AddTranslation("login_btn", "Login", "Войти");
            AddTranslation("password", "password", "Пароль");
            AddTranslation("login", "Email", "Email");
            AddTranslation("dictionaries", "Dictionaries", "Справочники");
            AddTranslation("users", "Users", "Пользователи");
            AddTranslation("roles", "Roles", "Роли");
            AddTranslation("role", "Role", "Роль");
            AddTranslation("create_btn", "Create", "Создать");
            AddTranslation("search_all_fields", "Search all fields", "Искать по всем полям");
            AddTranslation("reset_filters", "Reset filters", "Сбросить фильтры");
            AddTranslation("exit", "Exit", "Выйти");
            AddTranslation("translations", "Translations", "Локализация");
            AddTranslation("Administrator", "Administrator", "Администратор");
            AddTranslation("TransportCoordinator", "Transport Coordinator", "Транспортный координатор");
            AddTranslation("TransportCompanyEmployee", "Transport Company Employee", "Сотрудник транспортной компании");
            AddTranslation("permissions", "Permissions", "Разрешения");
            AddTranslation("name", "Name", "Наименование");
            AddTranslation("userName", "User name", "ФИО");
            AddTranslation("createCard", "Create", "Создание");
            AddTranslation("editCard", "Edit", "Редактирование");
            AddTranslation("totalCount", "{{count}} records", "{{count}} записей");
            AddTranslation("Are you sure to complete", "Are you sure to complete", "Выполнить действие");
            AddTranslation("for", "for", "для");
            AddTranslation("edit", "Edit", "Редактирование");
            AddTranslation("edit_btn", "Edit", "Редактировать");
            AddTranslation("information", "Information", "Информация");
            AddTranslation("position", "position", "Позиции");
            AddTranslation("returns", "returns", "Возвраты");
            AddTranslation("route", "route", "Маршрут");
            AddTranslation("accounts", "Accounts", "Счета");
            AddTranslation("reconciliation of expenses", "Reconciliation of expenses", "Сверка расходов");
            AddTranslation("temperature", "temperature", "Терморежим °С");
            AddTranslation("addressFrom", "addressFrom", "Адрес от");
            AddTranslation("addressTo", "addressTo", "Адрес до");
            AddTranslation("palletsCountGroup", "Pallets count", "Количество паллет");
            AddTranslation("boxesCountGroup", "Boxes count", "Количество коробок");
            AddTranslation("weigth", "weigth", "Вес, кг");
            AddTranslation("documents", "documents", "Документы");
            AddTranslation("history", "history", "История");
            AddTranslation("prepare", "prepare", "Предварительное");
            AddTranslation("plan", "plan", "Фактическое");
            AddTranslation("fact", "fact", "Подтверждённое");
            AddTranslation("planWeigth", "fact", "Плановый");
            AddTranslation("factWeigth", "fact", "Подтвержденный");
            AddTranslation("general info", "General info", "Общая информация");
            AddTranslation("from", "From", "От");
            AddTranslation("to", "To", "До");
            AddTranslation("representation", "Representation", "Представления");
            AddTranslation("Available", "Available", "Доступные поля");
            AddTranslation("Selected", "Selected", "Выбранные поля");
            AddTranslation("error_file_size", "error_file_size", "Размер файла не должен превышать 10Mb");
            AddTranslation("documentTypes", "Document types", "Типы документов");
            AddTranslation("vehicleTypes", "Vehicle types", "Типы ТС");
            AddTranslation("pickingTypes", "Picking types", "Типы комплектации");
            AddTranslation("emptyValue", "(empty)", "(пусто)");
            AddTranslation("fieldChanged", "Field {0} is set to {2}", "Значение поля {0} изменено на '{2}'");
            AddTranslation("documentAttached", "Document {0} is attached", "Добавлен документ {0}");
            AddTranslation("documentRemoved", "Document {0} is removed", "Удален документ {0}");

            /*start of add translates for action*/
            AddTranslation("createShipping", "Create shipping", "Создать перевозку");
            AddTranslation("unionOrders", "Union orders", "Объединить в перевозку");
            AddTranslation("cancelOrder", "Cancel order", "Отменить заказ");
            AddTranslation("removeFromShipping", "Remove From Shipping", "Убрать из перевозки");
            AddTranslation("saveOrder", "SaveOrder", "Сохранить");
            AddTranslation("saveOrders", "SaveOrders", "Сохранить заказы");
            AddTranslation("cancelOrders", "CancelOrders", "Отменить заказы");
            AddTranslation("createShippingForeach", "CreateShippingForEach", "Создать перевозку для каждого заказа");
            AddTranslation("orderShipped", "OrderShipped", "Заказ отгружен");
            AddTranslation("orderDelivered", "orderDelivered", "Заказ доставлен");
            AddTranslation("fullReject", "OrderShipped", "Полный возврат");
            AddTranslation("sendToArchive", "sendToArchive", "Перевести в архив");
            AddTranslation("testGenerateException", "TestGenerateException", "Сгенерировать ошибку в системе");
            AddTranslation("cancelShipping", "CancelShipping", "Отменить перевозку");
            AddTranslation("sendShippingToTk", "SendShippingToTk", "Отправить заявку в ТК");
            AddTranslation("cancelRequestShipping", "CancelRequestShipping", "Отменить заявку");
            AddTranslation("Create representation", "Create representation", "Создание представления");
            AddTranslation("Edit representation", "Edit representation {{name}}", "Редактирование представления {{name}}");
            AddTranslation("add value", "Add value", "Добавить значение");
            AddTranslation("create_orders", "Create order", "Создать заказ");
            AddTranslation("create_shippings", "Create shipping", "Создать перевозку");
            AddTranslation("new_orders", "New order", "Новый заказ");
            AddTranslation("new_shippings", "New shipping", "Новая перевозка");
            AddTranslation("edit_orders", "Order {{number}} - {{status}}", "Заказ {{number}} - {{status}}");
            AddTranslation("edit_shippings", "Shipping {{number}} - {{status}}", "Перевозка {{number}} - {{status}}");
            /*start of add translates for action*/

            /*start of add translates*/
            AddTranslation("login_welcome", "login_welcome", "TMS Beiersdorf");
            AddTranslation("login_support", "login_support", "TMS для компании Beiersdorf");
            AddTranslation("order", "Order", "Заказ");
            AddTranslation("orders", "Orders", "Заказы");
            AddTranslation("status", "Status", "Статус");
            AddTranslation("orderNumber", "Order No.", "Номер заказ клиента");
            AddTranslation("orderDate", "Order date", "Дата заказа");
            AddTranslation("orderType", "Order type", "Тип заказа");
            AddTranslation("payer", "Payer", "Плательщик");
            AddTranslation("clientName", "Client name", "Клиент");
            AddTranslation("temperatureMin", "Thermal Mode min °C", "Терморежим мин. °C");
            AddTranslation("temperatureMax", "Thermal Mode max °C", "Терморежим макс. °C");
            AddTranslation("soldTo", "Sold-to party", "Sold-to");
            AddTranslation("shippingDate", "Shipment date", "Дата отгрузки");
            AddTranslation("transitDays", "Days in transit", "Дней в пути");
            AddTranslation("deliveryDate", "Requested delivery date", "Дата доставки");
            AddTranslation("bDFInvoiceNumber", "Delivery No. BDF", "Номер накладной BDF");
            AddTranslation("articlesCount", "No. of materials", "Количество артикулов");
            AddTranslation("boxesCount", "Planned no. of boxes", "Предварительное количество коробок");
            AddTranslation("confirmedBoxesCount", "Confirmed no. of boxes", "Подтвержденное количество коробок");
            AddTranslation("palletsCount", "Planned no. of pallets", "Предварительное кол-во паллет");
            AddTranslation("confirmedPalletsCount", "Confirmed no. of pallets", "Подтвежденное количество паллет");
            AddTranslation("actualPalletsCount", "Actual no. of pallets", "Фактическое кол-во паллет");
            AddTranslation("confirmedBoxesCount", "Confirmed no. of boxes", "Подтвержденное кол-во коробок");
            AddTranslation("weightKg", "Planned weight, kg", "Плановый вес, кг");
            AddTranslation("actualWeightKg", "Actual weight, kg", "Фактический вес, кг");
            AddTranslation("orderAmountExcludingVAT", "Order value, excl. VAT, RUB", "Сумма заказа, без НДС");
            AddTranslation("invoiceAmountExcludingVAT", "Invoice value, excl. VAT", "Сумма по ТТН, без НДС");
            AddTranslation("deliveryRegion", "Region", "Регион");
            AddTranslation("deliveryity", "City", "Город");
            AddTranslation("shippingAddress", "Shipping WH address", "Адрес отгрузки");
            AddTranslation("deliveryAddress", "Delivery address", "Адрес доставки");
            AddTranslation("shippingStatus", "Shipping status", "Статус отгрузки");
            AddTranslation("deliveryStatus", "Delivery status", "Статус доставки");
            AddTranslation("clientAvisationTime", "Client's avisation time", "Время авизации у клиента");
            AddTranslation("orderComments", "Order comments", "Комментарии по заказу");
            AddTranslation("pickingTypeId", "Picking type", "Тип комплектации");
            AddTranslation("plannedArrivalTimeSlotBDFWarehouse", "Planned arriving time/Time-slot at BDF WH", "Плановое прибытие/тайм-слот (склад БДФ)");
            AddTranslation("loadingArrivalTime", "Arrival time to BDF WH", "Время прибытия на загрузку  (склад БДФ)");
            AddTranslation("loadingDepartureTime", "Departure time from BDF WH", "Время убытия со склада БДФ");
            AddTranslation("unloadingArrivalDate", "Actual arrival date to client", "Фактическая дата прибытия к грузополучателю");
            AddTranslation("unloadingArrivalTime", "Arrival time to client", "Время прибытия к грузополучателю");
            AddTranslation("unloadingDepartureDate", "Actual departure date from client", "Дата убытия от грузополучателя");
            AddTranslation("unloadingDepartureTime", "Departure time from client", "Время убытия от грузополучателя");
            AddTranslation("trucksDowntime", "Downtime", "Кол-во часов простоя машин");
            AddTranslation("returnInformation", "Returns info", "Информация по возвратам");
            AddTranslation("returnShippingAccountNo", "Invoice No. for return freight", "№ счета за перевозку возврата");
            AddTranslation("plannedReturnDate", "Planned return days", "Плановый срок возврата");
            AddTranslation("actualReturnDate", "Actual return days", "Фактический срок возврата");
            AddTranslation("majorAdoptionNumber", "Return act no. (Major)", "Номер приемного акта Мейджор");
            AddTranslation("orderCreationDate", "Order creation date", "Дата создания заказа в системе");
            AddTranslation("shippingId", "Shipping", "Перевозка");
            AddTranslation("orderState", "OrderState", "Статус заказа");
            AddTranslation("newOrderCreated", "New order created", "Создан новый заказ");
            AddTranslation("orderSetDraft", "Order {0} is not verified", "Заказ {0} не проверен");
            AddTranslation("orderSetCreated", "Order {0} is created", "Создан заказ {0}");
            AddTranslation("orderSetInShipping", "Order {0} is included in shipping {1}", "Заказ {0} включен в перевозку {1}");
            AddTranslation("orderSetShipped", "Order {0} is shipped", "Заказ {0} отгружен");
            AddTranslation("orderSetDelivered", "Order {0} is delivered", "Заказ {0} доставлен");
            AddTranslation("orderSetArchived", "Order {0} is archived", "Заказ {0} перенесен в архив");
            AddTranslation("orderSetLost", "Order {0} is lost", "Заказ {0} потерян");
            AddTranslation("orderSetFullReturn", "Full return of goods on order {0}", "Полный возврат товара по заказу {0}");
            AddTranslation("orderSetCancelled", "Order {0} is cancelled", "Заказ {0} отменен");
            AddTranslation("orderRemovedFromShipping", "Order {0} is removed from shipping {1}", "Заказ {0} удален из перевозки {1}");

            AddTranslation("vehicleEmpty", "Empty", "Не указан");
            AddTranslation("vehicleWaiting", "Waiting vehicle", "Ожидает ТС");
            AddTranslation("vehicleArrived", "Vehicle arrived", "ТС прибыло");
            AddTranslation("vehicleDepartured", "Vehicle departured", "ТС убыло");

            AddTranslation("draft", "Draft", "Не проверен");
            AddTranslation("canceled", "Canceled", "Отменён");
            AddTranslation("created", "Created", "Создан");
            AddTranslation("inShipping", "InShipping", "В перевозке");
            AddTranslation("shipped", "Shipped", "Отгружен");
            AddTranslation("delivered", "Delivered", "Доставлен");
            AddTranslation("archive", "Archive", "В архиве");
            AddTranslation("fullReturn", "FullReturn", "Полный возврат");
            AddTranslation("lost", "Lost", "Потерян");

            AddTranslation("shippingCreated", "Created", "Создана");
            AddTranslation("shippingCanceled", "Canceled", "Отменена");
            AddTranslation("shippingRequestSent", "ShippingRequestSent", "Заявка отправлена");
            AddTranslation("shippingConfirmed", "ShippingConfirmed", "Подтверждена");
            AddTranslation("shippingRejectedByTc", "ShippingRejectedByTc", "Отклонена ТК");
            AddTranslation("shippingCompleted", "ShippingCompleted", "Завершена");
            AddTranslation("shippingCompleted", "ShippingCompleted", "Завершена");
            AddTranslation("shippingBillSend", "ShippingBillSend", "Счёт выставлен");
            AddTranslation("shippingArhive", "ShippingArhive", "В архиве");
            AddTranslation("shippingProblem", "ShippingProblem", "Срыв поставки");

            AddTranslation("delivery", "Delivery", "Доставка");
            AddTranslation("selfDelivery", "Self delivery", "Самовывоз");

            AddTranslation("ltl", "LTL", "LTL");
            AddTranslation("ftl", "FTL", "FTL");

            AddTranslation("or", "OR", "OR");
            AddTranslation("fd", "FD", "FD");

            AddTranslation("createShipping", "createShipping", "Создать перевозку");
            AddTranslation("cancel", "cancel", "Отменить");
            AddTranslation("addButton", "Add", "Добавить");
            AddTranslation("removeFromShipping", "removeFromShipping", "Убрать из перевозки");
            AddTranslation("recordFactOfLoss", "recordFactOfLoss", "Заказ потерян");
            AddTranslation("shipping", "Shipping", "Перевозка");
            AddTranslation("shippings", "Shippings", "Перевозки");
            AddTranslation("shippingNumber", "Shipping number", "Номер перевозки");
            AddTranslation("deliveryType", "Delivery type", "Способ доставки");
            AddTranslation("temperatureMin", "Thermal Mode min °C", "Терморежим мин. °C");
            AddTranslation("temperatureMax", "Thermal Mode max °C", "Терморежим макс. °C");
            AddTranslation("tarifficationType", "Tariffication type", "Способ тарификации");
            AddTranslation("carrierId", "Carrier", "Транспортная компания");
            AddTranslation("vehicleTypeId", "Vehicle type", "Тип ТС");
            AddTranslation("palletsCount", "Planned no. of pallets", "Предварительное количество паллет");
            AddTranslation("actualPalletsCount", "Actual no. of pallets", "Фактическое количество паллет");
            AddTranslation("confirmedPalletsCount", "Confirmed no. of pallets", "Подтвержденное количество паллет");
            AddTranslation("weightKg", "Planned weight, kg", "Плановый вес, кг");
            AddTranslation("actualWeightKg", "Actual weight, kg", "Фактический вес, кг");
            AddTranslation("plannedArrivalTimeSlotBDFWarehouse", "Planned arriving time/Time-slot at BDF WH", "Плановое прибытие/тайм-слот (склад БДФ)");
            AddTranslation("loadingArrivalTime", "Arrival time to BDF WH", "Время прибытия на загрузку  (склад БДФ)");
            AddTranslation("loadingDepartureTime", "Departure time from BDF WH", "Время убытия со склада БДФ");
            AddTranslation("deliveryInvoiceNumber", "Delivery invoice number", "Номер счета за доставку");
            AddTranslation("deviationReasonsComments", "Comments (deviation from schedule reasons)", "Комментарии (причины отклонения от графика)");
            AddTranslation("totalDeliveryCost", "Total delivery cost", "Общая стоимость перевозки");
            AddTranslation("otherCosts", "Other", "Прочее");
            AddTranslation("deliveryCostWithoutVAT", "Delivery cost, excl. VAT", "Стоимость перевозки, без НДС");
            AddTranslation("returnCostWithoutVAT", "Delivery return cost, excl. VAT", "Стоимость перевозки возврата, без НДС");
            AddTranslation("invoiceAmountWithoutVAT", "Invoice value, excl. VAT", "Сумма по ТТН, без НДС");
            AddTranslation("additionalCostsWithoutVAT", "Additional costs, excl. VAT", "Дополнительные расходы на доставку, без НДС");
            AddTranslation("additionalCostsComments", "Additional costs comments", "Дополнительные расходы на доставку (комментарии)");
            AddTranslation("trucksDowntime", "Downtime", "Кол-во часов простоя машин");
            AddTranslation("returnRate", "Return rate", "Ставка за возврат");
            AddTranslation("additionalPointRate", "Additional point rate", "Ставка за дополнительную точку");
            AddTranslation("downtimeRate", "Downtime rate", "Ставка за простой");
            AddTranslation("blankArrivalRate", "Blank arrival rate", "Ставка за холостую подачу");
            AddTranslation("blankArrival", "Blank arrival", "Холостая подача");
            AddTranslation("waybill", "Waybill", "Транспортная накладная");
            AddTranslation("waybillTorg12", "Waybill Torg-12", "Товарная накладная(Торг-12)");
            AddTranslation("transportWaybill", "Waybill + Transport section", "Товарно-Транспортная накладная + Транспортный раздел");
            AddTranslation("invoice", "Invoice", "Счет-фактура");
            AddTranslation("documentsReturnDate", "Planning documents return date", "Плановая дата возврата документов");
            AddTranslation("actualDocumentsReturnDate", "Actual documents return date", "Фактическая дата возврата документов");
            AddTranslation("invoiceNumber", "Invoice number", "Номер счет-фактуры");
            AddTranslation("status", "Status", "Статус");
            AddTranslation("costsConfirmedByShipper", "Costs confirmed by shipper", "Расходы подтверждены грузоотправителем");
            AddTranslation("costsConfirmedByCarrier", "Costs confirmed by carrier", "Расходы подтверждены ТК");
            AddTranslation("shippingState", "ShippingState", "Статус перевозки");
            AddTranslation("shippingConfirmed", "Confirmed", "Подтверждена");
            AddTranslation("shippingCompleted", "Completed", "Завершена");
            AddTranslation("shippingSetCancelled", "Shipping {0} is cancelled", "Перевозка {0} отменена");

            AddTranslation("tariff", "Tariff", "Тариф");
            AddTranslation("tariffs", "Tariffs", "Тарифы");
            AddTranslation("tariff", "Tariff", "Тариф");
            AddTranslation("tariffs", "Tariffs", "Тарифы");
            AddTranslation("cityOfShipment", "CityOfShipment", "Город отгрузки");
            AddTranslation("deliveryCity", "DeliveryCity", "Город доставки");
            AddTranslation("billingMethod", "BillingMethod", "Способ тарификации");
            AddTranslation("transportCompany", "TransportCompany", "Транспортная компания");
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
            AddTranslation("warehouseName", "Warehouse name", "Наименование склада");
            AddTranslation("soldToNumber", "SoldToNumber", "SoldTo number");
            AddTranslation("region", "Region", "Регион");
            AddTranslation("city", "City", "Город");
            AddTranslation("address", "Address", "Адрес");
            AddTranslation("pickingTypeId", "Picking type", "Тип комплектации");
            AddTranslation("leadtimeDays", "LeadtimeDays", "Leadtime, дней");
            AddTranslation("customerWarehouse", "CustomerWarehouse", "Склад клиента");
            AddTranslation("usePickingType", "Fill order picking type", "Определение типа комплектации");
            AddTranslation("article", "Article", "Артикул");
            AddTranslation("articles", "Articles", "Артикулы");
            AddTranslation("sPGR", "SPGR", "SPGR");
            AddTranslation("description", "Description", "Описание");
            AddTranslation("nart", "Nart", "NART");
            AddTranslation("countryOfOrigin", "CountryOfOrigin", "Страна происхождения");
            AddTranslation("shelfLife", "ShelfLife", "Срок годности, дней");
            AddTranslation("status", "Status", "Статус");
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
            AddTranslation("transportCompany", "TransportCompany", "Транспортная компания");
            AddTranslation("transportCompanies", "TransportCompanies", "Транспортные компании");
            AddTranslation("title", "Title", "Название");
            AddTranslation("contractNumber", "ContractNumber", "Номер договора");
            AddTranslation("dateOfPowerOfAttorney", "DateOfPowerOfAttorney", "Дата доверенности");

            AddTranslation("Delete document", "Delete document {{name}}?", "Удалить докуьуте {{name}}?");
            AddTranslation("delete", "Delete", "Удалить");
            AddTranslation("download", "Download", "Скачать");
            AddTranslation("download", "Download", "Скачать");
            AddTranslation("Document name", "Document name", "Название документа");
            AddTranslation("Upload file", "Document name", "Загрузите файл");
            AddTranslation("Type", "Type", "Тип");
            AddTranslation("CancelButton", "Cancel", "Отменить");
            AddTranslation("Add document", "Add document", "Добавить документ");
            AddTranslation("Edit document", "Edit document", "Редактировать документ");
            AddTranslation("SaveButton", "Save", "Сохранить");
            AddTranslation("AddButton", "Add", "Добавить");
            AddTranslation("Create a Photo", "Create a Photo", "Сделать фотографию");
            AddTranslation("PhotoButton", "Photo", "Фото");
            AddTranslation("importFromExcel", "Import from Excel", "Загрузить из Excel");
            AddTranslation("exportExcel", "Export to Excel", "Выгрузить в Excel");
            /*end of add translates*/

        }

        private void AddTransportation(string from, string to)
        {
            Database.Insert("Transportations", new string[] { "Id", "From", "To" },
                    new string[] { (Guid.NewGuid()).ToString(), from, to });
        }

        private void AddOrder(string incoming)
        {
            Database.Insert("Orders", new string[] { "Id", "Incoming" },
                    new string[] { (Guid.NewGuid()).ToString(), incoming });
        }

        private string AddTranslation(string name, string en, string ru)
        {
            var id = (Guid.NewGuid()).ToString();
            Database.Insert("Translations", new string[] { "Id", "Name", "En", "Ru" },
                new string[] { id, name, en, ru });
            return id;
        }

        private string AddRole(string name)
        {
            var id = (Guid.NewGuid()).ToString();
            Database.Insert("Roles", new string[] { "Id", "Name" },
                new string[] { id, name });
            return id;
        }

        private void AddUser(string name, string roleid, string email, string passwordhash)
        {
            Database.Insert("Users", new string[] { "Id", "Name", "RoleId", "Email", "IsActive", "FieldsConfig", "PasswordHash" },
                new string[] { (Guid.NewGuid()).ToString(), name, roleid, email, "true", "", passwordhash });
        }

        private void AddTaskProperties(string taskName, string properties)
        {
            Database.Insert("TaskProperties", new string[] { "Id", "TaskName", "Properties" },
                new string[] { (Guid.NewGuid()).ToString(), taskName, properties });
        }

        private void AddDocumentType(string name)
        {
            Database.Insert("DocumentTypes", new string[] { "Id", "Name" },
                new string[] { (Guid.NewGuid()).ToString(), name });
        }
    }
}
