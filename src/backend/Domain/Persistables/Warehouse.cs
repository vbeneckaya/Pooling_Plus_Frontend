using System;
using Domain.Enums;

namespace Domain.Persistables
{   
    /// <summary>
    /// Склад
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
        /// SoldTo number
        /// </summary>
        public string SoldToNumber { get; set; }
        /// <summary>
        /// Регион
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Тип комплектации
        /// </summary>
        public Guid? PickingTypeId { get; set; }
        /// <summary>
        /// Leadtime, дней
        /// </summary>
        public int? LeadtimeDays { get; set; }
        /// <summary>
        /// Склад клиента
        /// </summary>
        public bool CustomerWarehouse { get; set; }
        /// <summary>
        /// Использование типа комплектации
        /// </summary>
        public bool UsePickingType { get; set; }
        /*end of fields*/
    }
}