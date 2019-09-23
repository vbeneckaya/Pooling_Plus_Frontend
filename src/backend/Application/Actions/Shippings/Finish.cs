using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;

namespace Application.Actions.Shippings
{
    public class Finish : IAppAction<Shipping>
    {
        public AppColor Color { get; set; }
        public AppActionResult Run(User user, Shipping target)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAvailable(Role role, Shipping target)
        {
            throw new System.NotImplementedException();
        }
    }
}