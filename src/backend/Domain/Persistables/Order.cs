using System;

namespace Domain.Persistables
{
    public class Order : IPersistable
    {
        public Guid Id { get; set; }
        /*end of fields*/
    }
}