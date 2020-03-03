using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202028021814)]
    public class AddReportPageNameForMobileToProvider : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Providers",  new Column("ReportPageNameForMobile",  DbType.String, ColumnProperty.Null));
        }
    }
}