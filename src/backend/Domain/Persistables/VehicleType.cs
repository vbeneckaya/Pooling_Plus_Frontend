using System;

namespace Domain.Persistables
{
    public class VehicleType : IPersistableWithName
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? TonnageId { get; set; }

        public Guid? BodyTypeId { get; set; }

        public int? PalletsCount { get; set; }

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
