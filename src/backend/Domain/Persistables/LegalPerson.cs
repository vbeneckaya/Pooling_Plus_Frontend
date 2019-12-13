using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Persistables
{
    /// <summary>
    /// Юр. лица
    /// </summary>
    public class LegalPerson : IPersistable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
