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
        public string TheNameOfTheWarehouse { get; set; }
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
        public string TypeOfEquipment { get; set; }
        /// <summary>
        /// Leadtime, дней
        /// </summary>
        public string LeadtimeDays { get; set; }
        /// <summary>
        /// Склад клиента
        /// </summary>
        public string CustomerWarehouse { get; set; }
        /// <summary>
        /// Использование типа комплектации
        /// </summary>
        public string UseTypeOfEquipment { get; set; }
        /*end of fields*/
    }
}