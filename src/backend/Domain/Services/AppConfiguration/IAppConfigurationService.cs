using System;

namespace Domain.Services.AppConfiguration
{
    public interface IAppConfigurationService : IService
    {
        AppConfigurationDto GetConfiguration();

        UserConfigurationDictionaryItem GetDictionaryConfiguration<TDto>(Guid? roleId);
    }
}