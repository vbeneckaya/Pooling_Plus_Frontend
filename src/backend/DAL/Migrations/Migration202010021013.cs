using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(202010021013)]
    public class AddPoolingSlotIdAndPoolingReservationId : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Shippings", new Column("PoolingSlotId",  DbType.String.WithSize(50)));
            Database.AddColumn("Shippings", new Column("PoolingReservationId",  DbType.String.WithSize(50)));
        }
    }
}