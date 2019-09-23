using System;
using Domain.Enums;

namespace Domain.Persistables
{   
    /// <summary>
    /// Перевозка
    /// </summary>
    public class Shipping : IPersistable
    {
        /// <summary>
        /// Db primary key
        /// </summary>    
        public Guid Id { get; set; }
        /// <summary>
        /// Номер перевозки
        /// </summary>
        public string TransportationNumber { get; set; }
        /// <summary>
        /// Способ доставки
        /// </summary>
        public string DeliveryMethod { get; set; }
        /// <summary>
        /// Терморежим
        /// </summary>
        public string ThermalMode { get; set; }
        /// <summary>
        /// Способ тарификации
        /// </summary>
        public string BillingMethod { get; set; }
        /// <summary>
        /// Транспортная компания
        /// </summary>
        public string TransportCompany { get; set; }
        /// <summary>
        /// Предварительное кол-во паллет
        /// </summary>
        public string PreliminaryNumberOfPallets { get; set; }
        /// <summary>
        /// Фактическое кол-во паллет
        /// </summary>
        public string ActualNumberOfPallets { get; set; }
        /// <summary>
        /// Подтвержденное кол-во паллет
        /// </summary>
        public string ConfirmedNumberOfPallets { get; set; }
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
        /// Номер счета за доставку
        /// </summary>
        public string DeliveryInvoiceNumber { get; set; }
        /// <summary>
        /// Комментарии (причины отклонения от графика)
        /// </summary>
        public string CommentsReasonsForDeviationFromTheSchedule { get; set; }
        /// <summary>
        /// Стоимость перевозки, без НДС
        /// </summary>
        public string TransportationCostWithoutVAT { get; set; }
        /// <summary>
        /// Стоимость перевозки возврата, без НДС
        /// </summary>
        public string ReturnShippingCostExcludingVAT { get; set; }
        /// <summary>
        /// Дополнительные расходы на доставку, без НДС
        /// </summary>
        public string AdditionalShippingCostsExcludingVAT { get; set; }
        /// <summary>
        /// Дополнительные расходы на доставку (комментарии)
        /// </summary>
        public string AdditionalShippingCostsComments { get; set; }
        /// <summary>
        /// Транспортная накладная
        /// </summary>
        public string Waybill { get; set; }
        /// <summary>
        /// Товарная накладная(Торг-12)
        /// </summary>
        public string WaybillTorg12 { get; set; }
        /// <summary>
        /// Товарно-Транспортная накладная +Транспортный раздел
        /// </summary>
        public string WaybillTransportSection { get; set; }
        /// <summary>
        /// Счет-фактура
        /// </summary>
        public string Invoice { get; set; }
        /// <summary>
        /// Фактическая дата возврата документов
        /// </summary>
        public string ActualReturnDate { get; set; }
        /// <summary>
        /// Номер счет-фактуры
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        public ShippingState Status { get; set; }
        /// <summary>
        /// Статус доставки
        /// </summary>
        public string DeliveryStatus { get; set; }
        /// <summary>
        /// Сумма подтверждена грузоотправителем
        /// </summary>
        public string AmountConfirmedByShipper { get; set; }
        /// <summary>
        /// Сумма подтверждена ТК
        /// </summary>
        public string AmountConfirmedByTC { get; set; }
        /*end of fields*/
    }
}