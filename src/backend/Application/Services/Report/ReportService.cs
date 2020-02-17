using System;
using Domain.Services.Report;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;

namespace Application.Services.Report
{
    public class ReportService : IReportService
    {
        
        public ReportService()
        {
        }
        public EmbeddedReportConfig GetConfig()
        {
            var oAuthResult = new PowerBiAuthenticator().AuthenticateAsync().GetAwaiter().GetResult();

            return GenerateReport(oAuthResult.AccessToken, Guid.Parse("8ac687eb-c107-4b44-a0ec-21aab341e752"), Guid.Parse("f9896a49-c76f-44f4-92ea-441c7662bd5f"));
        }
        
        public static EmbeddedReportConfig GenerateReport(string token, Guid groupId, Guid reportId)
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
                }

            }
            return config;
        }
    }
}