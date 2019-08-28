using System.Collections.Generic;
using Application.Shared;
using MongoDB.Bson;

namespace Application.Users
{
    public interface IUsersService : IService
    {
        IEnumerable<UserDto> GetAll(SearchForm form);
        ValidateResult SaveOrCreate(UserCreateUpdateFormDto userFrom);
        ValidateResult ChangeActivity(string userId);
        (VerificationResult Result, UserDto User) Get(ObjectId id);
        ValidateResult ChangePlatform(string platformId);
        IEnumerable<string> GetAvailablePlatformsIds();
        ValidateResult SaveFieldsConfig(string fieldsConfig);
        ValidateResult SaveUserParameter(string name, string value);
    }
}
