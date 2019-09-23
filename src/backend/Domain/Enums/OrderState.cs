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
        [StateColor(AppColor.Blue)]
        Draft,
        /// <summary>
        /// Отменён
        /// </summary>
        [StateColor(AppColor.Blue)]
        Canceled,
        /// <summary>
        /// Создан
        /// </summary>
        [StateColor(AppColor.Blue)]
        Created,
        /// <summary>
        /// В перевозке
        /// </summary>
        [StateColor(AppColor.Blue)]
        InShipping,
        /// <summary>
        /// Доставлен
        /// </summary>
        [StateColor(AppColor.Blue)]
        Delivered,
        /// <summary>
        /// В архиве
        /// </summary>
        [StateColor(AppColor.Blue)]
        Archive,
        /// <summary>
        /// Полный возврат
        /// </summary>
        [StateColor(AppColor.Blue)]
        FullReturn,
        /// <summary>
        /// Потерян
        /// </summary>
        [StateColor(AppColor.Blue)]
        Lost,
        /*end of fields*/
    }
}