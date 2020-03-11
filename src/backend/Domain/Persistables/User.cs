using System;

namespace Domain.Persistables
{
    public class User : IPersistableWithName
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public string FieldsConfig { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordToken { get; set; }
        public string Name { get; set; }

        public Role Role { get; set; }

        /// <summary>
        /// Транспортная компания
        /// </summary>
        public Guid? CarrierId { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public Guid? ClientId { get; set; }
        
        /// <summary>
        /// Поставщик
        /// </summary>
        public Guid? ProviderId { get; set; }

        /// <summary>
        /// Логин на пулинге
        /// </summary>
        public string PoolingLogin { get; set; }

        /// <summary>
        /// Пароль на пулинге
        /// </summary>
        public string PoolingPassword { get; set; }
        
        public string PoolingAccessToken { get; set; }
        public string PoolingRefreshToken { get; set; }


        /// <summary>
        /// Логин на FmCP
        /// </summary>
        public string FmCPLogin { get; set; }

        /// <summary>
        /// Пароль на FmCP
        /// </summary>
        public string FmCPPassword { get; set; }
        
        public string FmCPAccessToken { get; set; }
        public string FmCPRefreshToken { get; set; }


        public override string ToString()
        {
            return Name;
        }

        public bool IsPoolingIntegrated()
        {
            return !string.IsNullOrEmpty(PoolingLogin) && !string.IsNullOrEmpty(PoolingPassword);
        }
        public bool IsFMCPIntegrated()
        {
            return !string.IsNullOrEmpty(FmCPLogin) && !string.IsNullOrEmpty(FmCPPassword);
        }
    }
}