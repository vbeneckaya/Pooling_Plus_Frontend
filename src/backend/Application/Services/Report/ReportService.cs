using System;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.Report;
using Domain.Services.UserProvider;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;

namespace Application.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly ICommonDataService _dataService;
        private readonly IUserProvider _userProvider;

        public ReportService(ICommonDataService dataService, IUserProvider userProvider)
        {
            _dataService = dataService;
            _userProvider = userProvider;
        }
        public EmbeddedReportConfig GetConfig()
        {
            var user = _dataService.GetById<User>(_userProvider.GetCurrentUserId().Value);
            var role = _dataService.GetById<Role>(user.RoleId);
            Guid reportId;
            string reportPageNameForMobile;
            
            if (role.RoleType == Domain.Enums.RoleTypes.Administrator)
            {
                reportId = Guid.Parse("05ddf2c3-a2f4-4bdc-be06-e089b0aaa0c6");
                reportPageNameForMobile = "ReportSection";
            }
            else
            {
                var provider = _dataService.GetById<Provider>(user.ProviderId.Value);
                
                reportId = Guid.Parse(provider.ReportId);
                reportPageNameForMobile = provider.ReportPageNameForMobile;
            }
            
            var oAuthResult = new PowerBiAuthenticator().AuthenticateAsync().GetAwaiter().GetResult();

            return GenerateReport(oAuthResult.AccessToken, 
                Guid.Parse("8ac687eb-c107-4b44-a0ec-21aab341e752"), 
                reportId, reportPageNameForMobile);//"f9896a49-c76f-44f4-92ea-441c7662bd5f"
        }
        
        public static EmbeddedReportConfig GenerateReport(string token, Guid groupId, Guid reportId, string reportPageNameForMobile)
        {
            EmbeddedReportConfig config = null;
            var tokenCredentials = new TokenCredentials(token, "Bearer");

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            using (var client = new PowerBIClient(new Uri("https://api.powerbi.com"),tokenCredentials))
            {                
                Microsoft.PowerBI.Api.V2.Models.Report report = client.Reports.GetReportInGroup(groupId, reportId);
                if(report != null)
                {
                    var requestParameters = new GenerateTokenRequest();
                    requestParameters.AccessLevel = "View";

                    // Generate EmbedToken This function sends the POST message 
                    //with all parameters and returns the token
                    EmbedToken embedtoken = client.Reports.GenerateTokenInGroupAsync(
                        groupId,
                        reportId,
                        requestParameters).GetAwaiter().GetResult();

                    config = new EmbeddedReportConfig();
                    config.EmbedURL = report.EmbedUrl;
                    config.GroupID = groupId.ToString();
                    config.ReportID = reportId.ToString();
                    config.Token = embedtoken?.Token;
                    config.TokenID = embedtoken?.TokenId.ToString();
                    config.Expiration = embedtoken?.Expiration;
                    config.PageName = reportPageNameForMobile;
                }

            }
            return config;
        }
    }
}