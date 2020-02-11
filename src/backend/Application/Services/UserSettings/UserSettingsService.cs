using DAL;
using Domain.Persistables;
using Domain.Services.UserProvider;
using Domain.Services.UserSettings;
using Domain.Shared;
using System;
using System.Linq;

namespace Application.Services.UserSettings
{
    public class UserSettingsService : IUserSettingsService
    {
        public UserSettingDto GetValue(string key)
        {
            var userId = _userProvider.GetCurrentUserId();
            var entity = _db.UserSettings.FirstOrDefault(x => x.UserId == userId && x.Key == key);
            return new UserSettingDto
            {
                Key = key,
                Value = entity?.Value
            };
        }
        
        public UserSettingDto GetDefaultValue(string key)
        {
            var entity = _db.UserSettings.FirstOrDefault(x => x.UserId == null && x.Key == key);
            return new UserSettingDto
            {
                Key = key,
                Value = entity?.Value
            };
        }

        public ValidateResult SetValue(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return new ValidateResult("notFound");
            }

            var userId = _userProvider.GetCurrentUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            var entity = _db.UserSettings.FirstOrDefault(x => x.UserId == userId && x.Key == key);

            if (entity != null)
            {
                entity.Value = value;
            }
            else
            {
                entity = new UserSetting
                {
                    Id = Guid.NewGuid(),
                    UserId = userId.Value,
                    Key = key,
                    Value = value
                };
                _db.UserSettings.Add(entity);
            }

            _db.SaveChanges();

            return new ValidateResult(null, entity.Id.ToString());
        }
        
        public ValidateResult SetDefaultValue(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return new ValidateResult("notFound");
            }

            var entity = _db.UserSettings.FirstOrDefault(x => x.UserId == null &&x.Key == key);

            if (entity != null)
            {
                entity.Value = value;
            }
            else
            {
                entity = new UserSetting
                {
                    Id = Guid.NewGuid(),
                    UserId = null,
                    Key = key,
                    Value = value
                };
                _db.UserSettings.Add(entity);
            }

            _db.SaveChanges();

            return new ValidateResult(null, entity.Id.ToString());
        }

        public UserSettingsService(AppDbContext db, IUserProvider userProvider)
        {
            _db = db;
            _userProvider = userProvider;
        }

        private readonly AppDbContext _db;
        private readonly IUserProvider _userProvider;
    }
}
