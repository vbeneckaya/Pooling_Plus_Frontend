using Domain.Enums;
using System;

namespace Domain.Persistables
{
    /// <summary>
    /// Склад доставки
    /// </summary>
    public class Warehouse : IPersistable
    {
        /// <summary>
        /// Db primary key
        /// </summary>    
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование склада
        /// </summary>
        public string WarehouseName { get; set; }

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
        /// Адрес
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Распознанный адрес
        /// </summary>
        public string ValidAddress { get; set; }
        /// <summary>
        /// Нераспознанная часть
        /// </summary>
        public string UnparsedAddressParts { get; set; }
        /// <summary>
        /// Тип комплектации
        /// </summary>
        public Guid? PickingTypeId { get; set; }
        /// <summary>
        /// Leadtime, дней
        /// </summary>
        public int? LeadtimeDays { get; set; }
        /// <summary>
        /// Особенности комплектации
        /// </summary>
        //public string PickingFeatures { get; set; }
        /// <summary>
        /// Способ доставки
        /// </summary>
        public DeliveryType? DeliveryType { get; set; }
        /// <summary>
        /// Активный
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Время авизации
        /// </summary>
        public TimeSpan? AvisaleTime { get; set; }

        /// <summary>
        /// Юр. лицо
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public Guid? ClientId { get; set; }

        /// <summary>
        /// GLN
        /// </summary>
        public string Gln { get; set; }

        public override string ToString()
        {
            return Address;
        }
    }
}