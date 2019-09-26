using System;
using Domain.Enums;

namespace Domain.Persistables
{   
    /// <summary>
    /// Заказ
    /// </summary>
    public class Order : IPersistable
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
        public string SalesOrderNumber { get; set; }
        /// <summary>
        /// Дата заказа
        /// </summary>
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// Тип заказа
        /// </summary>
        public string TypeOfOrder { get; set; }
        /// <summary>
        /// Плательщик
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        /// Название клиента
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Sold-to
        /// </summary>
        public string SoldTo { get; set; }
        /// <summary>
        /// Дата отгрузки
        /// </summary>
        public DateTime? ShippingDate { get; set; }
        /// <summary>
        /// Дней в пути
        /// </summary>
        public int? DaysOnTheRoad { get; set; }
        /// <summary>
        /// Дата доставки
        /// </summary>
        public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// Номер накладной BDF
        /// </summary>
        public string BDFInvoiceNumber { get; set; }
        /// <summary>
        /// Номер счет-фактуры
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// Кол-во арт.
        /// </summary>
        public int? NumberOfArticles { get; set; }
        /// <summary>
        /// Предварительное Кол-во коробок
        /// </summary>
        public int? TheNumberOfBoxes { get; set; }
        /// <summary>
        /// Предварительное кол-во паллет
        /// </summary>
        public int? PreliminaryNumberOfPallets { get; set; }
        /// <summary>
        /// Фактическое кол-во паллет
        /// </summary>
        public int? ActualNumberOfPallets { get; set; }
        /// <summary>
        /// Подтвержденное кол-во коробок
        /// </summary>
        public int? ConfirmedBoxes { get; set; }
        /// <summary>
        /// Подтвержденное кол-во паллет
        /// </summary>
        public int? ConfirmedNumberOfPallets { get; set; }
        /// <summary>
        /// Вес, кг
        /// </summary>
        public decimal? WeightKg { get; set; }
        /// <summary>
        /// Сумма заказа, без НДС
        /// </summary>
        public decimal? OrderAmountExcludingVAT { get; set; }
        /// <summary>
        /// Сумма по ТТН, без НДС
        /// </summary>
        public decimal? TTNAmountExcludingVAT { get; set; }
        /// <summary>
        /// Регион
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Адрес отгрузки
        /// </summary>
        public string ShippingAddress { get; set; }
        /// <summary>
        /// Адрес доставки
        /// </summary>
        public string DeliveryAddress { get; set; }
        /// <summary>
        /// Время авизации у клиента
        /// </summary>
        public string CustomerAvizTime { get; set; }
        /// <summary>
        /// Комментарии по заказу
        /// </summary>
        public string OrderComments { get; set; }
        /// <summary>
        /// Тип комплектации
        /// </summary>
        public string TypeOfEquipment { get; set; }
        /// <summary>
        /// Плановое прибытие/тайм-слот (склад БДФ)
        /// </summary>
        public string PlannedArrivalTimeSlotBDFWarehouse { get; set; }
        /// <summary>
        /// Время прибытия на загрузку  (склад БДФ)
        /// </summary>
        public string ArrivalTimeForLoadingBDFWarehouse { get; set; }
        /// <summary>
        /// Время убытия со склада БДФ
        /// </summary>
        public string DepartureTimeFromTheBDFWarehouse { get; set; }
        /// <summary>
        /// Фактическая дата прибытия к грузополучателю
        /// </summary>
        public string ActualDateOfArrivalAtTheConsignee { get; set; }
        /// <summary>
        /// Время прибытия к грузополучателю
        /// </summary>
        public string ArrivalTimeToConsignee { get; set; }
        /// <summary>
        /// Дата убытия от грузополучателя
        /// </summary>
        public string DateOfDepartureFromTheConsignee { get; set; }
        /// <summary>
        /// Время убытия от грузополучателя
        /// </summary>
        public string DepartureTimeFromConsignee { get; set; }
        /// <summary>
        /// Кол-во часов простоя машин
        /// </summary>
        public string TheNumberOfHoursOfDowntime { get; set; }
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
        public string PlannedReturnDate { get; set; }
        /// <summary>
        /// Фактический срок возврата
        /// </summary>
        public string ActualReturnDate { get; set; }
        /// <summary>
        /// Номер приемного акта Мейджор
        /// </summary>
        public string MajorAdoptionNumber { get; set; }
        /// <summary>
        /// Авизация
        /// </summary>
        public string Avization { get; set; }
        /// <summary>
        /// Позиции в заказе
        /// </summary>
        public string OrderItems { get; set; }
        /// <summary>
        /// Дата создания заказа
        /// </summary>
        public string OrderCreationDate { get; set; }
        /// <summary>
        /// Перевозка
        /// </summary>
        public Guid? ShippingId { get; set; }
        /// <summary>
        /// ##comment##
        /// </summary>
        public string Positions { get; set; }
        /*end of fields*/
    }
}