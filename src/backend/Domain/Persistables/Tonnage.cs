using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Persistables
{
    public class Tonnage: IPersistableWithName
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Юр. лицо
        /// </summary>
        public Guid? CompanyId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
