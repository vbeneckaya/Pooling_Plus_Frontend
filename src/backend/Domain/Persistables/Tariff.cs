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
        public string CityOfShipment { get; set; }
        /// <summary>
        /// Город доставки
        /// </summary>
        public string DeliveryCity { get; set; }
        /// <summary>
        /// Способ тарификации
        /// </summary>
        public string BillingMethod { get; set; }
        /// <summary>
        /// Транспортная компания
        /// </summary>
        public string TransportCompany { get; set; }
        /// <summary>
        /// Тип ТС
        /// </summary>
        public string VehicleType { get; set; }
        /// <summary>
        /// Ставка FTL
        /// </summary>
        public string FTLBet { get; set; }
        /// <summary>
        /// Ставка LTL 1
        /// </summary>
        public string LTLRate1 { get; set; }
        /// <summary>
        /// Ставка LTL 2
        /// </summary>
        public string LTLRate2 { get; set; }
        /// <summary>
        /// Ставка LTL 3
        /// </summary>
        public string BetLTL3 { get; set; }
        /// <summary>
        /// Ставка LTL 4
        /// </summary>
        public string LTLRate4 { get; set; }
        /// <summary>
        /// Ставка LTL 5
        /// </summary>
        public string LTLRate5 { get; set; }
        /// <summary>
        /// Ставка LTL 6
        /// </summary>
        public string LTLRate6 { get; set; }
        /// <summary>
        /// Ставка LTL 7
        /// </summary>
        public string LTLRate7 { get; set; }
        /// <summary>
        /// Ставка LTL 8
        /// </summary>
        public string LTLBet8 { get; set; }
        /// <summary>
        /// Ставка LTL 9
        /// </summary>
        public string LTLRate9 { get; set; }
        /// <summary>
        /// Ставка LTL 10
        /// </summary>
        public string LTLRate10 { get; set; }
        /// <summary>
        /// Ставка LTL 11
        /// </summary>
        public string LTLRate11 { get; set; }
        /// <summary>
        /// Ставка LTL 12
        /// </summary>
        public string LTLRate12 { get; set; }
        /// <summary>
        /// Ставка LTL 13
        /// </summary>
        public string LTLRate13 { get; set; }
        /// <summary>
        /// Ставка LTL 14
        /// </summary>
        public string LTLRate14 { get; set; }
        /// <summary>
        /// Ставка LTL 15
        /// </summary>
        public string BetLTL15 { get; set; }
        /// <summary>
        /// Ставка LTL 16
        /// </summary>
        public string LTLRate16 { get; set; }
        /// <summary>
        /// Ставка LTL 17
        /// </summary>
        public string BetLTL17 { get; set; }
        /// <summary>
        /// Ставка LTL 18
        /// </summary>
        public string BetLTL18 { get; set; }
        /// <summary>
        /// Ставка LTL 19
        /// </summary>
        public string LTLRate19 { get; set; }
        /// <summary>
        /// Ставка LTL 20
        /// </summary>
        public string LTLRate20 { get; set; }
        /// <summary>
        /// Ставка LTL 21
        /// </summary>
        public string LTLRate21 { get; set; }
        /// <summary>
        /// Ставка LTL 22
        /// </summary>
        public string LTLRate22 { get; set; }
        /// <summary>
        /// Ставка LTL 23
        /// </summary>
        public string LTLRate23 { get; set; }
        /// <summary>
        /// Ставка LTL 24
        /// </summary>
        public string LTLRate24 { get; set; }
        /// <summary>
        /// Ставка LTL 25
        /// </summary>
        public string LTLRate25 { get; set; }
        /// <summary>
        /// Ставка LTL 26
        /// </summary>
        public string LTLBet26 { get; set; }
        /// <summary>
        /// Ставка LTL 27
        /// </summary>
        public string LTLRate27 { get; set; }
        /// <summary>
        /// Ставка LTL 28
        /// </summary>
        public string LTLBet28 { get; set; }
        /// <summary>
        /// Ставка LTL 29
        /// </summary>
        public string LTLRate29 { get; set; }
        /// <summary>
        /// Ставка LTL 30
        /// </summary>
        public string LTLRate30 { get; set; }
        /// <summary>
        /// Ставка LTL 31
        /// </summary>
        public string LTLRate31 { get; set; }
        /// <summary>
        /// Ставка LTL 32
        /// </summary>
        public string BetLTL32 { get; set; }
        /// <summary>
        /// Ставка LTL 33
        /// </summary>
        public string LTLBid33 { get; set; }
        /*end of fields*/
    }
}