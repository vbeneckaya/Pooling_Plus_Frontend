using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(2020_05_03_12_54)]
    public class AddReportIdToClientsAndTransportCompanies : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Clients", new Column("ReportId", DbType.String));
            Database.AddColumn("TransportCompanies", new Column("ReportId", DbType.String));
        }
    }
}