using System;
using System.Collections.Generic;
using Domain.Enums;
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
            var t = RoleTypes.Administrator;
            foreach (TEnum value in values)
            {
                var num =  (int)Enum.Parse(typeof(TEnum), value.ToString());
                string name = value.ToString().ToLowerFirstLetter();
                result.Add(new LookUpDto
                {
                    Value = num.ToString(),
                    Name = name.Translate(currentUser.Language) 
                });
            }

            return result;
        }
    }
}