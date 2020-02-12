using Domain.Enums;
using Domain.Extensions;
using System;

namespace Domain.Persistables
{
    /// <summary>
    /// Перевозка
    /// </summary>
    public class Shipping : IPersistable, IWithDocumentsPersistable
    {
        /// <summary>
        /// Db primary key
        /// </summary>    
        [IgnoreHistory]
        public Guid Id { get; set; }
        /// <summary>
        /// Пользователь создавший перевозку
        /// </summary>    
        [IgnoreHistory]
        public Guid UserCreatorId { get; set; }
        /// <summary>
        /// Номер перевозки
        /// </summary>
        public string ShippingNumber { get; set; }
        /// <summary>
        /// Способ доставки
        /// </summary>
        public DeliveryType? DeliveryType { get; set; }
        /// <summary>
        /// Терморежим мин. °C
        /// </summary>
        public int? TemperatureMin { get; set; }
        /// <summary>
        /// Терморежим макс. °C
        /// </summary>
        public int? TemperatureMax { get; set; }
        /// <summary>
        /// Способ тарификации
        /// </summary>
        public TarifficationType? TarifficationType { get; set; }
        /// <summary>
        /// Транспортная компания
        /// </summary>
        [ReferenceType(typeof(TransportCompany))]
        public Guid? CarrierId { get; set; }
        /// <summary>
        /// Поставщик
        /// </summary>
        [ReferenceType(typeof(Provider))]
        public Guid? ProviderId { get; set; }
        /// <summary>
        /// Тип ТС
        /// </summary>
        [ReferenceType(typeof(VehicleType))]
        public Guid? VehicleTypeId { get; set; }

        /// <summary>
        /// Тип кузова
        /// </summary>
        [ReferenceType(typeof(BodyType))]
        public Guid? BodyTypeId { get; set; }

        /// <summary>
        /// Плановое кол-во паллет
        /// </summary>
        public int? PalletsCount { get; set; }
        /// <summary>
        /// Плановое кол-во паллет введено вручную
        /// </summary>
        [IgnoreHistory]
        public bool ManualPalletsCount { get; set; }
        /// <summary>
        /// Подтвержденное кол-во паллет
        /// </summary>
        public int? ConfirmedPalletsCount { get; set; }
        /// <summary>
        /// Подтвержденное кол-во паллет введено вручную
        /// </summary>
        [IgnoreHistory]
        public bool ManualConfirmedPalletsCount { get; set; }
        /// <summary>
        /// Плановый вес, кг
        /// </summary>
        public decimal? WeightKg { get; set; }
        /// <summary>
        /// Плановый вес введен вручную
        /// </summary>
        [IgnoreHistory]
        public bool ManualWeightKg { get; set; }
        /// <summary>
        /// Подтвержденный вес, кг
        /// </summary>
        public decimal? ConfirmedWeightKg { get; set; }
        /// <summary>
        /// Подтвержденный вес введен вручную
        /// </summary>
        [IgnoreHistory]
        public bool ManualConfirmedWeightKg { get; set; }
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
        /// Номер счета за доставку
        /// </summary>
        public string DeliveryInvoiceNumber { get; set; }
        /// <summary>
        /// Комментарии (причины отклонения от графика)
        /// </summary>
        public string DeviationReasonsComments { get; set; }
        /// <summary>
        /// Общая стоимость перевозки
        /// </summary>
        public decimal? TotalDeliveryCost { get; set; }
        /// <summary>
        /// Общая стоимость перевозки введена вручную
        /// </summary>
        [IgnoreHistory]
        public bool ManualTotalDeliveryCost { get; set; }
        /// <summary>
        /// Прочее
        /// </summary>
        public decimal? OtherCosts { get; set; }
        /// <summary>
        /// Стоимость перевозки, без НДС
        /// </summary>
        public decimal? DeliveryCostWithoutVAT { get; set; }
        /// <summary>
        /// Стоимость перевозки возврата, без НДС
        /// </summary>
        public decimal? ReturnCostWithoutVAT { get; set; }
        /// <summary>
        /// Сумма по ТТН, без НДС
        /// </summary>
        public decimal? InvoiceAmountWithoutVAT { get; set; }
        /// <summary>
        /// Дополнительные расходы на доставку, без НДС
        /// </summary>
        public decimal? AdditionalCostsWithoutVAT { get; set; }
        /// <summary>
        /// Дополнительные расходы на доставку (комментарии)
        /// </summary>
        public string AdditionalCostsComments { get; set; }
        /// <summary>
        /// Кол-во часов простоя машин
        /// </summary>
        public decimal? TrucksDowntime { get; set; }
        /// <summary>
        /// Кол-во часов простоя машин введено вручную
        /// </summary>
        [IgnoreHistory]
        public bool ManualTrucksDowntime { get; set; }
        /// <summary>
        /// Ставка за возврат
        /// </summary>
        public decimal? ReturnRate { get; set; }
        /// <summary>
        /// Ставка за дополнительную точку
        /// </summary>
        public decimal? AdditionalPointRate { get; set; }
        /// <summary>
        /// Ставка за простой
        /// </summary>
        public decimal? DowntimeRate { get; set; }
        /// <summary>
        /// Ставка за холостую подачу
        /// </summary>
        public decimal? BlankArrivalRate { get; set; }
        /// <summary>
        /// Холостая подача
        /// </summary>
        public bool BlankArrival { get; set; }
        /// <summary>
        /// Транспортная накладная
        /// </summary>
        public bool Waybill { get; set; }
        /// <summary>
        /// Товарная накладная(Торг-12)
        /// </summary>
        public bool WaybillTorg12 { get; set; }
        /// <summary>
        /// Товарно-Транспортная накладная +Транспортный раздел
        /// </summary>
        public bool TransportWaybill { get; set; }
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
        /// Номер счет-фактуры
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        [IgnoreHistory]
        public ShippingState? Status { get; set; }
        /// <summary>
        /// Pooling статус
        /// </summary>
        [IgnoreHistory]
        public ShippingPoolingState? PoolingState { get; set; }
        /// <summary>
        /// Pooling информация
        /// </summary>
        public string PoolingInfo { get; set; }
        /// <summary>
        /// id слота на Pooling
        /// </summary>
        public string PoolingSlotId { get; set; }
        /// <summary>
        /// id брони на Pooling
        /// </summary>
        public string PoolingReservationId { get; set; }
        /// <summary>
        /// Расходы подтверждена грузоотправителем
        /// </summary>
        public bool CostsConfirmedByShipper { get; set; }
        /// <summary>
        /// Расходы подтверждена ТК
        /// </summary>
        public bool CostsConfirmedByCarrier { get; set; }
        /// <summary>
        /// Дата создания перевозки
        /// </summary>
        [IgnoreHistory]
        public DateTime? ShippingCreationDate { get; set; }
        
        [IgnoreHistory]
        public bool ManualTarifficationType { get; set; }

        /// <summary>
        /// Водитель
        /// </summary>
        public string Driver { get; set; }
        
        /// <summary>
        /// Номер ТС
        /// </summary>
        public string VehicleNumber { get; set; }
        
        /// <summary>
        /// Маршрут
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Id заявки на портале
        /// </summary>
        public string FmcpWaybillId { get; set; }

        public override string ToString()
        {
            return ShippingNumber;
        }
    }
}