using Domain.Extensions;

namespace Domain.Enums
{
    /// <summary>
    /// Статус заказа
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// Создан
        /// </summary>
        [StateColor(AppColor.Blue), OrderNumber(1)]
        Created = 1,
        
        /// <summary>
        /// Отменён
        /// </summary>
        [StateColor(AppColor.Brown), OrderNumber(2)]
        Canceled = 2,
        
        /// <summary>
        /// В перевозке
        /// </summary>
        [StateColor(AppColor.Purple), OrderNumber(3)]
        InShipping = 3,

        /// <summary>
        /// Отгружен
        /// </summary>
        [StateColor(AppColor.Orange), OrderNumber(4)]
        Shipped = 4,

        /// <summary>
        /// Доставлен
        /// </summary>
        [StateColor(AppColor.Green), OrderNumber(5)]
        Delivered = 5,

        /// <summary>
        /// В архиве
        /// </summary>
        [StateColor(AppColor.Teal), OrderNumber(6)]
        Archive = 6,

        /// <summary>
        /// Полный возврат
        /// </summary>
        [StateColor(AppColor.Olive), OrderNumber(7)]
        FullReturn = 7,

        /// <summary>
        /// Потерян
        /// </summary>
        [StateColor(AppColor.Red), OrderNumber(8)]
        Lost = 8,
    }
}