using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202022011242)]
    public class ReplaceShipmentCityAndDeliveryCityInTarriffs : Migration
    {
        public override void Apply()
        {
            Database.RemoveColumn("Tariffs", "ShipmentCity");
            Database.RemoveColumn("Tariffs", "DeliveryCity");

            Database.AddColumn("Tariffs", new Column("ShippingWarehouseId", DbType.Guid));
            Database.AddColumn("Tariffs", new Column("DeliveryWarehouseId", DbType.Guid));
        }
    }
}