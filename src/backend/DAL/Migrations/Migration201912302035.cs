using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(201912302035)]
    public class AddNewColumnsToOrders : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Orders", new Column("ProductTypeId", DbType.Guid));
            Database.AddColumn("Orders", new Column("ItemsNumber", DbType.Int32));
            Database.AddColumn("Orders", new Column("ShippingWarehouseGln", DbType.String));
            Database.AddColumn("Orders", new Column("DeliveryWarehouseGln", DbType.String));
            Database.AddColumn("Orders", new Column("ShippingRegion", DbType.String));
        }
    }
}
