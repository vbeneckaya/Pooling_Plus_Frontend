using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(202001221038)]
    public class AddPoolingStateAndInfo : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Shippings", new Column("PoolingState", DbType.Int32));
            Database.AddColumn("Shippings", new Column("PoolingInfo", DbType.String.WithSize(250)));
        }
    }
}