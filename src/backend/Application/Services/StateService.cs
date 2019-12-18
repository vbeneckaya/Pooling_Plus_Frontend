using Domain.Extensions;
using Domain.Services;
using Domain.Shared;
using System.Collections.Generic;

namespace Application.Services
{
    public class StateService : IStateService
    {
        public IEnumerable<StateDto> GetAll<TEnum>()
        {
            var values = Domain.Extensions.Extensions.GetOrderedEnum<TEnum>();
            var result = new List<StateDto>();
            foreach (var value in values)
            {
                string name = value.ToString().ToLowerFirstLetter();
                result.Add(new StateDto
                {
                    Name = name,
                    Value = name,
                    Color = value.GetColor().ToString().ToLowerFirstLetter()
                });
            }
            return result;
        }

        public IEnumerable<LookUpDto> ForSelect<TEnum>()
        {
            var values = Domain.Extensions.Extensions.GetOrderedEnum<TEnum>();
            var result = new List<LookUpDto>();
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
