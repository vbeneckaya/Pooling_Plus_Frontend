using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202031011503)]
    public class AddProviderIdIntoOrdersAndShippingsTable : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Orders", new Column("ProviderId", DbType.Guid));
            Database.AddColumn("Shippings", new Column("ProviderId", DbType.Guid));
        }
    }
}