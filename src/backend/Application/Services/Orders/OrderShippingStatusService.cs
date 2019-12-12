using Domain.Enums;
using Domain.Extensions;
using Domain.Services.Orders;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using System.Collections.Generic;

namespace Application.Services.Orders
{
    public class OrderShippingStatusService : IOrderShippingStatusService
    {
        private readonly IUserProvider _userProvider;

        public OrderShippingStatusService(IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        public IEnumerable<StateDto> GetAll()
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            var values = Domain.Extensions.Extensions.GetOrderedEnum<ShippingState>();

            var result = new List<StateDto>();
            result.Add(new StateDto
            {
                Name = "emptyValue".Translate(lang),
                Value = LookUpDto.EmptyValue,
                Color = null
            });

            foreach (var value in values)
            {
                string name = value.ToString().ToLowerFirstLetter();
                result.Add(new StateDto
                {
                    Name = name.Translate(lang),
                    Value = name,
                    Color = value.GetColor().ToString().ToLowerFirstLetter()
                });
            }

            return result;
        }

        public IEnumerable<LookUpDto> ForSelect()
        {
            var lang = _userProvider.GetCurrentUser()?.Language;

            var values = Domain.Extensions.Extensions.GetOrderedEnum<ShippingState>();

            var result = new List<LookUpDto>();
            result.Add(new LookUpDto
            {
                Name = "emptyValue".Translate(lang),
                Value = LookUpDto.EmptyValue,
                IsFilterOnly = true
            });

            foreach (var value in values)
            {
                result.Add(new LookUpDto
                {
                    Name = value.ToString().ToLowerFirstLetter(),
                    Value = value.ToString()
                });
            }

            return result;
        }
    }
}
