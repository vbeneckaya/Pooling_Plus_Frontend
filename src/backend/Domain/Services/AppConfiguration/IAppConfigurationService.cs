namespace Domain.Services.AppConfiguration
{
    public interface IAppConfigurationService : IService
    {
        AppConfigurationDto GetConfiguration();
    }
}