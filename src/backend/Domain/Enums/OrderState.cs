using Domain.Extensions;

namespace Domain.Enums
{
    public enum OrderState
    {
        [StateColor(AppColor.Blue)]
        Draft,
        [StateColor(AppColor.Blue)]
        Canceled,
        [StateColor(AppColor.Blue)]
        Created,
        [StateColor(AppColor.Blue)]
        InShipping,
        [StateColor(AppColor.Blue)]
        Delivered,
        [StateColor(AppColor.Blue)]
        Archive,
        [StateColor(AppColor.Blue)]
        FullReturn,
        [StateColor(AppColor.Blue)]
        Lost,
        /*end of fields*/
    }
}