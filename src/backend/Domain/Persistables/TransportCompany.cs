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
        public string Title { get; set; }
        public string ContractNumber { get; set; }
        public string DateOfPowerOfAttorney { get; set; }
        /*end of fields*/
    }
}