using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(202001160332)]
    public class AddDriverAngVehicleNumberInShippings : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Shippings", new Column("Driver", DbType.String));
            Database.AddColumn("Shippings", new Column("VehicleNumber", DbType.String.WithSize(50)));
        }
    }
}