using Domain.Extensions;

namespace Domain.Enums
{
    public enum RoleTypes
    {
        /// <summary>
        /// Администратор
        /// </summary>
        [OrderNumber(0)]
        Administrator = 1,

        /// <summary>
        /// Клиент
        /// </summary>
        [OrderNumber(1)]
        Client = 2,

        /// <summary>
        /// Транспортная компания
        /// </summary>
        [OrderNumber(2)]
        TransportCompany = 3,

        /// <summary>
        /// Поставщик
        /// </summary>
        [OrderNumber(3)]
        Provider = 4,

    }
}
