using System;

namespace Domain.Persistables
{
    public class User : IPersistableWithName
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string FieldsConfig { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }

        public Role Role { get; set; }

        /// <summary>
        /// Транспортная компания
        /// </summary>
        public Guid? CarrierId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}