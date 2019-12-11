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
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Наименование склада
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Распознанный адрес
        /// </summary>
        public string ValidAddress { get; set; }
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
        /// Активный
        /// </summary>
        public bool IsActive { get; set; }

        public override string ToString()
        {
            return WarehouseName;
        }
    }
}
