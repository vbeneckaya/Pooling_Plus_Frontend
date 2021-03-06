using Domain.Enums;
using Domain.Extensions;
using System;

namespace Domain.Persistables
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class Order : IPersistableWithCreator, IWithDocumentsPersistable
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
        public Guid? UserCreatorId { get; set; }        
        /// <summary>
        /// Статус
        /// </summary>
        [IgnoreHistory]
        public OrderState Status { get; set; }
        /// <summary>
        /// Номер накладной
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// Клиент
        /// </summary>
        [ReferenceType(typeof(Client))]
        public Guid? ClientId { get; set; }
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
        /// Номер заказ клиента
        /// </summary>
        public string ClientOrderNumber { get; set; }
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
        /// Дата отгрузки введена вручную
        /// </summary>
        [IgnoreHistory]
        public bool ManualShippingDate { get; set; }
        /// <summary>
        /// Дней в пути
        /// </summary>
        public int? TransitDays { get; set; }
        /// <summary>
        /// Дата доставки
        /// </summary>
        public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// Дата доставки введена вручную
        /// </summary>
        [IgnoreHistory]
        public bool ManualDeliveryDate { get; set; }
        /// <summary>
        /// Кол-во арт.
        /// </summary>
        public int? ArticlesCount { get; set; }
        /// <summary>
        /// Плановое Кол-во коробок
        /// </summary>
        public decimal? BoxesCount { get; set; }
        /// <summary>
        /// Подтвежденное количество коробок
        /// </summary>
        public decimal? ConfirmedBoxesCount { get; set; }
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
        /// Подтвежденное кол-во паллет
        /// </summary>
        public int? ConfirmedPalletsCount { get; set; }
        /// <summary>
        /// Плановый вес, кг
        /// </summary>
        public decimal? WeightKg { get; set; }
        /// <summary>
        /// Подтвержденныей вес, кг
        /// </summary>
        public decimal? ConfirmedWeightKg { get; set; }
        /// <summary>
        /// Сумма заказа, без НДС
        /// </summary>
        public decimal? OrderAmountExcludingVAT { get; set; }
        /// <summary>
        /// Сумма по ТТН, без НДС
        /// </summary>
        public decimal? InvoiceAmountExcludingVAT { get; set; }
        /// <summary>
        /// Город отгрузки
        /// </summary>
        public string ShippingCity { get; set; }
        /// <summary>
        /// Регион доставки
        /// </summary>
        public string DeliveryRegion { get; set; }
        /// <summary>
        /// Город доставки
        /// </summary>
        public string DeliveryCity { get; set; }
        /// <summary>
        /// Адрес отгрузки
        /// </summary>
        [IgnoreHistory]
        public string ShippingAddress { get; set; }
        /// <summary>
        /// Адрес доставки
        /// </summary>
        [IgnoreHistory]
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
        /// Время авизации на складе отгрузки
        /// </summary>
        public TimeSpan? ShippingAvisationTime { get; set; }
        /// <summary>
        /// Время авизации у клиента
        /// </summary>
        public TimeSpan? ClientAvisationTime { get; set; }
        /// <summary>
        /// Время авизации у клиента выбрано вручную
        /// </summary>
        [IgnoreHistory]
        public bool ManualClientAvisationTime { get; set; }
        /// <summary>
        /// Комментарии по заказу
        /// </summary>
        public string OrderComments { get; set; }
        /// <summary>
        /// Тип комплектации
        /// </summary>
        [ReferenceType(typeof(PickingType))]
        public Guid? PickingTypeId { get; set; }
        /// <summary>
        /// Тип комплектации выбран вручнуюt
        /// </summary>
        [IgnoreHistory]
        public bool ManualPickingTypeId { get; set; }
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
        [IgnoreHistory]
        [ReferenceType(typeof(Shipping))]
        public Guid? ShippingId { get; set; }
        /// <summary>
        /// Номер перевозки
        /// </summary>
        public string ShippingNumber { get; set; }
        /// <summary>
        /// Статус перевозки
        /// </summary>
        [IgnoreHistory]
        public ShippingState? OrderShippingStatus { get; set; }
        /// <summary>
        /// Склад отгрузки
        /// </summary>
        [ReferenceType(typeof(ShippingWarehouse))]
        public Guid? ShippingWarehouseId { get; set; }
        /// <summary>
        /// Склад доставки
        /// </summary>
        [IgnoreHistory]
        [ReferenceType(typeof(Warehouse))]
        public Guid? DeliveryWarehouseId { get; set; }

        /// <summary>
        /// Активный?
        /// </summary>
        [IgnoreHistory]
        public bool IsActive { get; set; } = true;
        /*end of fields*/

        public override string ToString()
        {
            return OrderNumber;
        }

        /// <summary>
        /// Дата изменения
        /// </summary>
        [IgnoreHistory]
        public DateTime? OrderChangeDate { get; set; }

        /// <summary>
        /// Заказ подтвержден
        /// </summary>
        public bool OrderConfirmed { get; set; }

        /// <summary>
        /// Статус возврата документов
        /// </summary>
        public bool DocumentReturnStatus { get; set; }

        /// <summary>
        /// Источник данных в заказе (список инжекций)
        /// </summary>
        [IgnoreHistory]
        public string Source { get; set; }

        /// <summary>
        /// Особенности комплектации
        /// </summary>
        public string PickingFeatures { get; set; }

        /// <summary>
        /// Способ доставки
        /// </summary>
        public DeliveryType? DeliveryType { get; set; }

        /// <summary>
        /// Комментарий (причины отклонения от графика)
        /// </summary>
        public string DeviationsComment { get; set; }

        /// <summary>
        /// Базовая стоимость, без НДС
        /// </summary>
        public decimal? DeliveryCost { get; set; }

        /// <summary>
        /// Базовая стоимость введена вручную
        /// </summary>
        [IgnoreHistory]
        public bool ManualDeliveryCost { get; set; }

        /// <summary>
        /// Фактическая стоимость, без НДС
        /// </summary>
        public decimal? ActualDeliveryCost { get; set; }

        /// <summary>
        /// Способ тарификации
        /// </summary>
        public TarifficationType? TarifficationType { get; set; }

        /// <summary>
        /// Тип ТС
        /// </summary>
        [ReferenceType(typeof(VehicleType))]
        public Guid? VehicleTypeId { get; set; }

        /// <summary>
        /// Количество штук в накладной
        /// </summary>
        public int? ItemsNumber { get; set; }

        /// <summary>
        /// Тип продукта
        /// </summary>
        public Guid? ProductTypeId { get; set; }

        /// <summary>
        /// GLN (Склад отгрузки)
        /// </summary>
        public string ShippingWarehouseGln { get; set; }

        /// <summary>
        /// GLN (Склад доставки)
        /// </summary>
        public string DeliveryWarehouseGln { get; set; }

        /// <summary>
        /// Регион отгрузки
        /// </summary>
        public string ShippingRegion { get; set; }

        /// <summary>
        /// ID РЦ
        /// </summary>
        public string DistributionCenterId { get; set; }
        
        /// <summary>
        /// Поклажедатель
        /// </summary>
        public string Depositor { get; set; } 
    }
}