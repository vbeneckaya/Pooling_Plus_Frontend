using Domain.Extensions;

namespace Domain.Enums
{
    /// <summary>
    /// Статус ТС
    /// </summary>
    public enum VehicleState
    {
        /// <summary>
        /// Не задан
        /// </summary>
        [StateColor(AppColor.Black)]
        VehicleEmpty = 0,
        /// <summary>
        /// Ожидает ТС
        /// </summary>
        [StateColor(AppColor.Grey)]
        VehicleWaiting = 1,
        /// <summary>
        /// ТС прибыло
        /// </summary>
        [StateColor(AppColor.Orange)]
        VehicleArrived = 2,
        /// <summary>
        /// ТС убыло
        /// </summary>
        [StateColor(AppColor.Green)]
        VehicleDepartured = 3
    }
}
