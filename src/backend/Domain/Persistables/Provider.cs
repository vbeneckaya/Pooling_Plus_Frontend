using System;

namespace Domain.Persistables
{
    /// <summary>
    /// Поставщик
    /// </summary>
    public class Provider : IPersistable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public string Inn { get; set; }
        
        public string Cpp { get; set; }
        
        public string LegalAddress { get; set; }
        
        public string ActualAddress { get; set; }
        
        public string ContactPerson { get; set; }
        
        public string ContactPhone { get; set; }
        
        public string Email { get; set; }
        public string ReportId { get; set; }		

        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            return Name;
        }
    }
}
