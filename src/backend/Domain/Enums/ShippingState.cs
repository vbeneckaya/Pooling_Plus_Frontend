using Domain.Extensions;

namespace Domain.Enums
{
    public enum ShippingState
    {
        [StateColor(AppColor.Blue)]
        Canceled,
        [StateColor(AppColor.Blue)]
        Created,
        [StateColor(AppColor.Blue)]
        Confirmed,
        [StateColor(AppColor.Blue)]
        Completed,
        /*end of fields*/
    }
}