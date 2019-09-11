using System;

namespace Domain.Persistables
{
    public class Transportation : IPersistable
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}