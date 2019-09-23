using Domain.Extensions;

namespace Domain.Enums
{
    /// <summary>
    /// Статус перевозки
    /// </summary>
    public enum ShippingState
    {
        /// <summary>
        /// Отменена
        /// </summary>
        [StateColor(AppColor.Blue)]
        Canceled,
        /// <summary>
        /// Создана
        /// </summary>
        [StateColor(AppColor.Blue)]
        Created,
        /// <summary>
        /// Подтверждена
        /// </summary>
        [StateColor(AppColor.Blue)]
        Confirmed,
        /// <summary>
        /// Завершена
        /// </summary>
        [StateColor(AppColor.Blue)]
        Completed,
        /*end of fields*/
    }
}