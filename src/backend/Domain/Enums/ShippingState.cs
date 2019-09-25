using Domain.Extensions;

namespace Domain.Enums
{
    /// <summary>
    /// Статус перевозки
    /// </summary>
    public enum ShippingState
    {
        /// <summary>
        /// Создана
        /// </summary>
        [StateColor(AppColor.Blue)]
        ShippingCreated,
        /// <summary>
        /// Отменена
        /// </summary>
        [StateColor(AppColor.Red)]
        ShippingCanceled,
        /// <summary>
        /// Заявка отправлена
        /// </summary>
        [StateColor(AppColor.Purple)]
        ShippingRequestSent,
        /// <summary>
        /// Подтверждена
        /// </summary>
        [StateColor(AppColor.Green)]
        ShippingConfirmed,
        /// <summary>
        /// Отклонена ТК
        /// </summary>
        [StateColor(AppColor.Grey)]
        ShippingRejectedByTc,
        /// <summary>
        /// Завершена
        /// </summary>
        [StateColor(AppColor.Blue)]
        ShippingCompleted,
        /// <summary>
        /// Счёт выставлен
        /// </summary>
        [StateColor(AppColor.Olive)]
        ShippingBillSend,
        /// <summary>
        /// В архиве
        /// </summary>
        [StateColor(AppColor.Teal)]
        ShippingArhive,
        /// <summary>
        /// Срыв поставки
        /// </summary>
        [StateColor(AppColor.Brown)]
        ShippingProblem,
        /*end of fields*/
    }
}