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
        [StateColor(AppColor.Red)]
        ShippingCanceled,
        /// <summary>
        /// Создана
        /// </summary>
        [StateColor(AppColor.Blue)]
        ShippingCreated,
        /// <summary>
        /// Подтверждена
        /// </summary>
        [StateColor(AppColor.Purple)]
        ShippingConfirmed,
        /// <summary>
        /// Завершена
        /// </summary>
        [StateColor(AppColor.Green)]
        ShippingCompleted,
        /*end of fields*/
    }
}