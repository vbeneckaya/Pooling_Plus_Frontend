using Domain.Extensions;

namespace Domain.Enums
{
    /// <summary>
    /// Статус пулинга перевозки
    /// </summary>
    public enum ShippingPoolingState
    {
        /// <summary>
        /// Pooling доступен
        /// </summary>
        [StateColor(AppColor.Orange), OrderNumber(0)]
        PoolingAvailable = 0,

        /// <summary>
        /// Забронировано
        /// </summary>
        [StateColor(AppColor.Blue), OrderNumber(1)]
        PoolingBooked = 1,

        /// <summary>
        /// Проблема
        /// </summary>
        [StateColor(AppColor.Red), OrderNumber(2)]
        PoolingProblem = 2,
    }
}