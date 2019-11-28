using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(201911271755)]
    public class AddAvisaleTimeToOrders : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Orders", new Column("AvisaleTime", DbType.Time, ColumnProperty.Null));
        }
    }
}
