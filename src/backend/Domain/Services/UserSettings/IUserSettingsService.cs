using Domain.Shared;

namespace Domain.Services.UserSettings
{
    public interface IUserSettingsService
    {
        UserSettingDto GetValue(string key);
        ValidateResult SetValue(string key, string value);
        
        UserSettingDto GetDefaultValue(string key);
        ValidateResult SetDefaultValue(string key, string value);
    }
}
