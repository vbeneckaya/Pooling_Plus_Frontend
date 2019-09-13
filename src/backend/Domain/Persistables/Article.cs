using System;

namespace Domain.Persistables
{
    public class Article : IPersistable
    {
        public Guid Id { get; set; }
        /*end of fields*/
    }
}