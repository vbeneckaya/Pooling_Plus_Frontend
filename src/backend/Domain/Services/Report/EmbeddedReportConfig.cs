using System;

namespace Domain.Services.Report
{
    public class EmbeddedReportConfig
    {
        public string EmbedURL { get; set; }
        public string ReportID { get; set; }
        public string PageName { get; set; }
        public string GroupID { get; set; }
        public string Token { get; set; }
        public string TokenID { get; set; }
        public DateTime? Expiration { get; set; }

    }
}