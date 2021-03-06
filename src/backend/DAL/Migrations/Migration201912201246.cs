using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(201912201246)]
    public class RemovePickingFeatiresFromWarehouses : Migration
    {
        public override void Apply()
        {
            Database.RemoveColumn("Warehouses", "PickingFeatures");
            Database.RemoveColumn("Warehouses", "SoldToNumber");
            Database.AddColumn("Warehouses", new Column("ClientId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("Warehouses", new Column("Gln", DbType.String));
        }
    }
}