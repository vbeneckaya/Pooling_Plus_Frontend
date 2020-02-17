using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(201912161335)]
    public class AddCompanyIdToOrdersAndShippings : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Orders", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("Shippings", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
        }
    }
}