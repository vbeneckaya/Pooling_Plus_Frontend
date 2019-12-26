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
        /// Юр. лицо
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// Pooling ID
        /// </summary>
        public string PoolingId { get; set; }

        /// <summary>
        /// Активен
        /// </summary>
        public bool IsActive { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}