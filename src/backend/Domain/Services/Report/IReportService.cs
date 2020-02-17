namespace Domain.Services.Report
{
    public interface IReportService : IService
    {
        EmbeddedReportConfig GetConfig();
    }
}