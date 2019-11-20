using System;
using Domain.Enums;

namespace Domain.Persistables
{   
    /// <summary>
    /// Транспортная компания
    /// </summary>
    public class TransportCompany : IPersistable
    {
        /// <summary>
        /// Db primary key
        /// </summary>    
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Номер договора
        /// </summary>
        public string ContractNumber { get; set; }

        /// <summary>
        /// Дата доверенности
        /// </summary>
        public string DateOfPowerOfAttorney { get; set; }

        /// <summary>
        /// Активен
        /// </summary>
        public bool IsActive { get; set; }
    }
}