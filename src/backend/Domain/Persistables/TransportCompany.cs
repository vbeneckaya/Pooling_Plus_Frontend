using System;

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
        /// Pooling ID
        /// </summary>
        public string PoolingId { get; set; }
        
        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }
        
        /// <summary>
        /// КПП
        /// </summary>
        public string Cpp { get; set; }
        
        /// <summary>
        /// Юридический адрес
        /// </summary>
        public string LegalAddress { get; set; }
        
        /// <summary>
        /// Фактический адрес
        /// </summary>
        public string ActualAddress { get; set; }
        
        /// <summary>
        /// Контактное лицо
        /// </summary>
        public string ContactPerson { get; set; }
        
        /// <summary>
        /// Контактный телефон
        /// </summary>
        public string ContactPhone { get; set; }
        
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Активен
        /// </summary>
        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            return Title;
        }
    }
}