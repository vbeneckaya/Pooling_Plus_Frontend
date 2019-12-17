using Domain.Extensions;

namespace Domain.Enums
{
    public enum RolePermissions
    {
        // Reserved value
        None = 0,

        /// <summary>
        /// Просмотр заказов
        /// </summary>
        [OrderNumber(0)]
        OrdersView = 1,

        /// <summary>
        /// Создание заказов
        /// </summary>
        [OrderNumber(1)]
        OrdersCreate = 2,

        /// <summary>
        /// Просмотр и прикрепление документов к заказу
        /// </summary>
        [OrderNumber(2)]
        OrdersViewAndAttachDocument = 4,

        /// <summary>
        /// Редактирование и удаление документов из заказа
        /// </summary>
        [OrderNumber(3)]
        OrdersEditAndDeleteDocument = 5,

        /// <summary>
        /// Просмотр истории заказов
        /// </summary>
        [OrderNumber(4)]
        OrdersViewHistory = 6,

        /// <summary>
        /// Просмотр перевозок
        /// </summary>
        [OrderNumber(5)]
        ShippingsView = 7,

        /// <summary>
        /// Просмотр и прикрепление документов к перевозке
        /// </summary>
        [OrderNumber(6)]
        ShippingsViewAndAttachDocument = 10,

        /// <summary>
        /// Редактирование и удаление документов из перевозки
        /// </summary>
        [OrderNumber(7)]
        ShippingsEditAndDeleteDocument = 11,

        /// <summary>
        /// Просмотр истории перевозок
        /// </summary>
        [OrderNumber(8)]
        ShippingsViewHistory = 12,

        /// <summary>
        /// Просмотр тарифов
        /// </summary>
        //[OrderNumber(9)]
        //TariffsView = 13,

        /// <summary>
        /// Редактирование тарифов
        /// </summary>
        //[OrderNumber(10)]
        //TariffsEdit = 14,

        /// <summary>
        /// Редактирование складов доставки
        /// </summary>
        [OrderNumber(12)]
        WarehousesEdit = 15,

        /// <summary>
        /// Редактирование артикулов
        /// </summary>
        //[OrderNumber(13)]
        //ArticlesEdit = 16,

        /// <summary>
        /// Редактирование типов комплектаций
        /// </summary>
        [OrderNumber(14)]
        PickingTypesEdit = 17,

        /// <summary>
        /// Редактирование транспортных компаний
        /// </summary>
        [OrderNumber(15)]
        TransportCompaniesEdit = 18,

        /// <summary>
        /// Редактирование типов ТС
        /// </summary>
        [OrderNumber(16)]
        VehicleTypesEdit = 19,

        /// <summary>
        /// Редактирование типов документов
        /// </summary>
        [OrderNumber(17)]
        DocumentTypesEdit = 20,

        /// <summary>
        /// Редактирование ролей
        /// </summary>
        [OrderNumber(18)]
        RolesEdit = 21,

        /// <summary>
        /// Редактирование пользователей
        /// </summary>
        [OrderNumber(19)]
        UsersEdit = 22,

        /// <summary>
        /// Настройка полей
        /// </summary>
        [OrderNumber(20)]
        FieldsSettings = 23,

        /// <summary>
        /// Редактирование складов отгрузки
        /// </summary>
        [OrderNumber(11)]
        ShippingWarehousesEdit = 24,
    }
}
