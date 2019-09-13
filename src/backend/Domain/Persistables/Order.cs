using System;

namespace Domain.Persistables
{
    public class Order : IPersistable
    {
        public Guid Id { get; set; }
        public string IncomingNumber { get; set; }
        /*end of fields*/
    }
}