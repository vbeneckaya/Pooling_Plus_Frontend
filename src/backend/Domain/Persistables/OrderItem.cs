using System;

namespace Domain.Persistables
{
    /// <summary>
    /// Позиция заказа
    /// </summary>
    public class OrderItem : IPersistable
    {
        /// <summary>
        /// Db primary key
        /// </summary>    
        public Guid Id { get; set; }

        /// <summary>
        /// ID Заказа
        /// </summary>    
        public Guid OrderId { get; set; }

        /// <summary>
        /// NART
        /// </summary>
        public string Nart { get; set; }

        /// <summary>
        /// Количество единиц товара
        /// </summary>
        public int Quantity { get; set; }
    }
}
