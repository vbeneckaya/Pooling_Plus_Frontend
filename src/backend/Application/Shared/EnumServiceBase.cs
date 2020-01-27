using System;
using System.Collections.Generic;
using Domain.Extensions;
using Domain.Services;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;

namespace Application.Shared
{
    public abstract class EnumServiceBase<TEnum> : IEnumService<TEnum> where TEnum : Enum
    {
        protected readonly IUserProvider _userIdProvider;
        
        protected EnumServiceBase(
            IUserProvider userIdProvider)
        {
            _userIdProvider = userIdProvider;
        }
        
        public IEnumerable<LookUpDto> ForSelect()
        {
            var currentUser = _userIdProvider.GetCurrentUser();
            var values = Domain.Extensions.Extensions.GetOrderedEnum<TEnum>();
            var result = new List<LookUpDto>();
            foreach (var value in values)
            {
                string name = value.ToString().ToLowerFirstLetter();
                result.Add(new LookUpDto
                {
                    Value = name,
                    Name = name.Translate(currentUser.Language) 
                });
            }

            return result;
        }
    }
}