using Domain.Extensions;

namespace Domain.Enums
{
    /// <summary>
    /// Статус заказа
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// Не подтверждён
        /// </summary>
        [StateColor(AppColor.Grey)]
        Draft,
        /// <summary>
        /// Отменён
        /// </summary>
        [StateColor(AppColor.Brown)]
        Canceled,
        /// <summary>
        /// Создан
        /// </summary>
        [StateColor(AppColor.Blue)]
        Created,
        /// <summary>
        /// В перевозке
        /// </summary>
        [StateColor(AppColor.Purple)]
        InShipping,
        /// <summary>
        /// Отгружен
        /// </summary>
        [StateColor(AppColor.Orange)]
        Shipped,
        /// <summary>
        /// Доставлен
        /// </summary>
        [StateColor(AppColor.Green)]
        Delivered,
        /// <summary>
        /// В архиве
        /// </summary>
        [StateColor(AppColor.Teal)]
        Archive,
        /// <summary>
        /// Полный возврат
        /// </summary>
        [StateColor(AppColor.Olive)]
        FullReturn,
        /// <summary>
        /// Потерян
        /// </summary>
        [StateColor(AppColor.Red)]
        Lost,

        /// <summary>
        /// Подтверждён
        /// </summary>
        [StateColor(AppColor.Green)]
        Confirmed,
        /*end of fields*/
    }
}