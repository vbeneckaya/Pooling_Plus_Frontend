using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202002171305)]
    public class AddDepositorToOrders : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Orders", new Column("Depositor",  DbType.String));
        }
    }
}
