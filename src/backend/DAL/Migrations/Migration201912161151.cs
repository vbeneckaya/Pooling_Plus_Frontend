using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(201912161151)]
    public class Migration201912161151 : Migration
    {
        public override void Apply()
        {
            Database.RemoveColumn("ShippingWarehouses", "Code");
            Database.RemoveColumn("ShippingWarehouses", "ValidAddress");

            Database.AddColumn("ShippingWarehouses", new Column("Gln", DbType.String));
            Database.AddColumn("ShippingWarehouses", new Column("Settlement", DbType.String));
            Database.AddColumn("ShippingWarehouses", new Column("Block", DbType.String.WithSize(100)));
            Database.AddColumn("ShippingWarehouses", new Column("RegionId", DbType.String.WithSize(100)));
            Database.AddColumn("ShippingWarehouses", new Column("UnparsedParts", DbType.String.WithSize(1000)));
        }
    }
}
