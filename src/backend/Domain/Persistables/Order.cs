using Domain.Enums;
using System;

namespace Domain.Persistables
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class Order : IWithDocumentsPersistable, IPersistable
    {
        /// <summary>
        /// Db primary key
        /// </summary>    
        public Guid Id { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        public OrderState Status { get; set; }
        /// <summary>
        /// Номер заказ клиента
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// Дата заказа
        /// </summary>
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// Тип заказа
        /// </summary>
        public OrderType? OrderType { get; set; }
        /// <summary>
        /// Плательщик
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        /// Название клиента
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// Sold-to
        /// </summary>
        public string SoldTo { get; set; }
        /// <summary>
        /// Терморежим мин. °C
        /// </summary>
        public int? TemperatureMin { get; set; }
        /// <summary>
        /// Терморежим макс. °C
        /// </summary>
        public int? TemperatureMax { get; set; }
        /// <summary>
        /// Дата отгрузки
        /// </summary>
        public DateTime? ShippingDate { get; set; }
        /// <summary>
        /// Дней в пути
        /// </summary>
        public int? TransitDays { get; set; }
        /// <summary>
        /// Дата доставки
        /// </summary>
        public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// Номер накладной BDF
        /// </summary>
        public string BdfInvoiceNumber { get; set; }
        /// <summary>
        /// Кол-во арт.
        /// </summary>
        public int? ArticlesCount { get; set; }
        /// <summary>
        /// Предварительное Кол-во коробок
        /// </summary>
        public decimal? BoxesCount { get; set; }
        /// <summary>
        /// Подтвержденное количество коробок
        /// </summary>
        public decimal? ConfirmedBoxesCount { get; set; }
        /// <summary>
        /// Предварительное кол-во паллет
        /// </summary>
        public int? PalletsCount { get; set; }
        /// <summary>
        /// Предварительное кол-во паллет введено вручную
        /// </summary>
        public bool ManualPalletsCount { get; set; }
        /// <summary>
        /// Подтвежденное кол-во паллет
        /// </summary>
        public int? ConfirmedPalletsCount { get; set; }
        /// <summary>
        /// Фактическое кол-во паллет
        /// </summary>
        public int? ActualPalletsCount { get; set; }
        /// <summary>
        /// Плановый вес, кг
        /// </summary>
        public decimal? WeightKg { get; set; }
        /// <summary>
        /// Фактический вес, кг
        /// </summary>
        public decimal? ActualWeightKg { get; set; }
        /// <summary>
        /// Сумма заказа, без НДС
        /// </summary>
        public decimal? OrderAmountExcludingVAT { get; set; }
        /// <summary>
        /// Сумма по ТТН, без НДС
        /// </summary>
        public decimal? InvoiceAmountExcludingVAT { get; set; }
        /// <summary>
        /// Регион
        /// </summary>
        public string DeliveryRegion { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public string DeliveryCity { get; set; }
        /// <summary>
        /// Адрес отгрузки
        /// </summary>
        public string ShippingAddress { get; set; }
        /// <summary>
        /// Адрес доставки
        /// </summary>
        public string DeliveryAddress { get; set; }
        /// <summary>
        /// Статус отгрузки
        /// </summary>
        public VehicleState ShippingStatus { get; set; }
        /// <summary>
        /// Статус доставки
        /// </summary>
        public VehicleState DeliveryStatus { get; set; }
        /// <summary>
        /// Время авизации у клиента
        /// </summary>
        public TimeSpan? ClientAvisationTime { get; set; }
        /// <summary>
        /// Комментарии по заказу
        /// </summary>
        public string OrderComments { get; set; }
        /// <summary>
        /// Тип комплектации
        /// </summary>
        public Guid? PickingTypeId { get; set; }
        /// <summary>
        /// Плановое прибытие/тайм-слот (склад БДФ)
        /// </summary>
        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }
        /// <summary>
        /// Время прибытия на загрузку  (склад БДФ)
        /// </summary>
        public DateTime? LoadingArrivalTime { get; set; }
        /// <summary>
        /// Время убытия со склада БДФ
        /// </summary>
        public DateTime? LoadingDepartureTime { get; set; }
        /// <summary>
        /// Фактическая дата прибытия к грузополучателю
        /// </summary>
        public DateTime? UnloadingArrivalTime { get; set; }
        /// <summary>
        /// Дата убытия от грузополучателя
        /// </summary>
        public DateTime? UnloadingDepartureTime { get; set; }
        /// <summary>
        /// Кол-во часов простоя машин
        /// </summary>
        public decimal? TrucksDowntime { get; set; }
        /// <summary>
        /// Информация по возвратам
        /// </summary>
        public string ReturnInformation { get; set; }
        /// <summary>
        /// № счета за перевозку возврата
        /// </summary>
        public string ReturnShippingAccountNo { get; set; }
        /// <summary>
        /// Плановый срок возврата
        /// </summary>
        public DateTime? PlannedReturnDate { get; set; }
        /// <summary>
        /// Фактический срок возврата
        /// </summary>
        public DateTime? ActualReturnDate { get; set; }
        /// <summary>
        /// Номер приемного акта Мейджор
        /// </summary>
        public string MajorAdoptionNumber { get; set; }
        /// <summary>
        /// Дата создания заказа
        /// </summary>
        public DateTime? OrderCreationDate { get; set; }
        /// <summary>
        /// Товарная накладная(Торг-12)
        /// </summary>
        public bool WaybillTorg12 { get; set; }
        /// <summary>
        /// Счет-фактура
        /// </summary>
        public bool Invoice { get; set; }
        /// <summary>
        /// Плановая дата возврата документов
        /// </summary>
        public DateTime? DocumentsReturnDate { get; set; }
        /// <summary>
        /// Фактическая дата возврата документов
        /// </summary>
        public DateTime? ActualDocumentsReturnDate { get; set; }
        /// <summary>
        /// Перевозка
        /// </summary>
        public Guid? ShippingId { get; set; }
        /// <summary>
        /// Номер перевозки
        /// </summary>
        public string ShippingNumber { get; set; }
        /// <summary>
        /// Статус перевозки
        /// </summary>
        public ShippingState? OrderShippingStatus { get; set; }
        /// <summary>
        /// Склад отгрузки
        /// </summary>
        public Guid? ShippingWarehouseId { get; set; }
        /// <summary>
        /// Склад доставки
        /// </summary>
        public Guid? DeliveryWarehouseId { get; set; }
        /// <summary>
        /// Активный?
        /// </summary>
        public bool IsActive { get; set; }
        /*end of fields*/

        public override string ToString()
        {
            return OrderNumber;
        }
    }
}