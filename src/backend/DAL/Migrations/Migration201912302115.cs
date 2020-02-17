using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(201912302115)]
    public class AddDistributionCenterIdToOrders : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Orders", new Column("DistributionCenterId", DbType.String));
        }
    }
}
