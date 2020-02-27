using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202026021930)]
    public class AddReportIdToProvider : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Providers",  new Column("ReportId",  DbType.String, ColumnProperty.Null));
        }
    }
}