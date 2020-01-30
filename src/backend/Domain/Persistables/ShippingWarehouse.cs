using System;

namespace Domain.Persistables
{
    /// <summary>
    /// Склад отгрузки
    /// </summary>
    public class ShippingWarehouse : IPersistable
    {
        /// <summary>
        /// Db primary key
        /// </summary>    
        public Guid Id { get; set; }
        /// <summary>
        /// GLN
        /// </summary>
        public string Gln { get; set; }
        /// <summary>
        /// Наименование склада
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Индекс
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// Регион
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// Район
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// Населенный пункт
        /// </summary>
        public string Settlement { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Улица
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// Дом
        /// </summary>
        public string House { get; set; }
        /// <summary>
        /// Корпус/строение
        /// </summary>
        public string Block { get; set; }
        /// <summary>
        /// Нераспознанная часть
        /// </summary>
        public string UnparsedParts { get; set; }
        /// <summary>
        /// ID региона
        /// </summary>
        public string RegionId { get; set; }
        /// <summary>
        /// ID клиента
        /// </summary>
        public Guid ProviderId { get; set; }
        /// <summary>
        /// Активный
        /// </summary>
        public bool IsActive { get; set; }

        public override string ToString()
        {
            return Address;
        }
    }
}
