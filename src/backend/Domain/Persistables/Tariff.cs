using System;
using Domain.Enums;

namespace Domain.Persistables
{   
    /// <summary>
    /// Тариф
    /// </summary>
    public class Tariff : IPersistable
    {
        /// <summary>
        /// Db primary key
        /// </summary>    
        public Guid Id { get; set; }
        /// <summary>
        /// Город отгрузки
        /// </summary>
        public string ShipmentCity { get; set; }
        /// <summary>
        /// Город доставки
        /// </summary>
        public string DeliveryCity { get; set; }
        /// <summary>
        /// Способ тарификации
        /// </summary>
        public TarifficationType? TarifficationType { get; set; }
        /// <summary>
        /// Транспортная компания
        /// </summary>
        public Guid? CarrierId { get; set; }
        /// <summary>
        /// Тип ТС
        /// </summary>
        public Guid? VehicleTypeId { get; set; }
        /// <summary>
        /// Ставка FTL
        /// </summary>
        public decimal? FTLRate { get; set; }
        /// <summary>
        /// Ставка LTL 1
        /// </summary>
        public decimal? LTLRate1 { get; set; }
        /// <summary>
        /// Ставка LTL 2
        /// </summary>
        public decimal? LTLRate2 { get; set; }
        /// <summary>
        /// Ставка LTL 3
        /// </summary>
        public decimal? LTLRate3 { get; set; }
        /// <summary>
        /// Ставка LTL 4
        /// </summary>
        public decimal? LTLRate4 { get; set; }
        /// <summary>
        /// Ставка LTL 5
        /// </summary>
        public decimal? LTLRate5 { get; set; }
        /// <summary>
        /// Ставка LTL 6
        /// </summary>
        public decimal? LTLRate6 { get; set; }
        /// <summary>
        /// Ставка LTL 7
        /// </summary>
        public decimal? LTLRate7 { get; set; }
        /// <summary>
        /// Ставка LTL 8
        /// </summary>
        public decimal? LTLRate8 { get; set; }
        /// <summary>
        /// Ставка LTL 9
        /// </summary>
        public decimal? LTLRate9 { get; set; }
        /// <summary>
        /// Ставка LTL 10
        /// </summary>
        public decimal? LTLRate10 { get; set; }
        /// <summary>
        /// Ставка LTL 11
        /// </summary>
        public decimal? LTLRate11 { get; set; }
        /// <summary>
        /// Ставка LTL 12
        /// </summary>
        public decimal? LTLRate12 { get; set; }
        /// <summary>
        /// Ставка LTL 13
        /// </summary>
        public decimal? LTLRate13 { get; set; }
        /// <summary>
        /// Ставка LTL 14
        /// </summary>
        public decimal? LTLRate14 { get; set; }
        /// <summary>
        /// Ставка LTL 15
        /// </summary>
        public decimal? LTLRate15 { get; set; }
        /// <summary>
        /// Ставка LTL 16
        /// </summary>
        public decimal? LTLRate16 { get; set; }
        /// <summary>
        /// Ставка LTL 17
        /// </summary>
        public decimal? LTLRate17 { get; set; }
        /// <summary>
        /// Ставка LTL 18
        /// </summary>
        public decimal? LTLRate18 { get; set; }
        /// <summary>
        /// Ставка LTL 19
        /// </summary>
        public decimal? LTLRate19 { get; set; }
        /// <summary>
        /// Ставка LTL 20
        /// </summary>
        public decimal? LTLRate20 { get; set; }
        /// <summary>
        /// Ставка LTL 21
        /// </summary>
        public decimal? LTLRate21 { get; set; }
        /// <summary>
        /// Ставка LTL 22
        /// </summary>
        public decimal? LTLRate22 { get; set; }
        /// <summary>
        /// Ставка LTL 23
        /// </summary>
        public decimal? LTLRate23 { get; set; }
        /// <summary>
        /// Ставка LTL 24
        /// </summary>
        public decimal? LTLRate24 { get; set; }
        /// <summary>
        /// Ставка LTL 25
        /// </summary>
        public decimal? LTLRate25 { get; set; }
        /// <summary>
        /// Ставка LTL 26
        /// </summary>
        public decimal? LTLRate26 { get; set; }
        /// <summary>
        /// Ставка LTL 27
        /// </summary>
        public decimal? LTLRate27 { get; set; }
        /// <summary>
        /// Ставка LTL 28
        /// </summary>
        public decimal? LTLRate28 { get; set; }
        /// <summary>
        /// Ставка LTL 29
        /// </summary>
        public decimal? LTLRate29 { get; set; }
        /// <summary>
        /// Ставка LTL 30
        /// </summary>
        public decimal? LTLRate30 { get; set; }
        /// <summary>
        /// Ставка LTL 31
        /// </summary>
        public decimal? LTLRate31 { get; set; }
        /// <summary>
        /// Ставка LTL 32
        /// </summary>
        public decimal? LTLRate32 { get; set; }
        /// <summary>
        /// Ставка LTL 33
        /// </summary>
        public decimal? LTLRate33 { get; set; }
        /*end of fields*/
    }
}