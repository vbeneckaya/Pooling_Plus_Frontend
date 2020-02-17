using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(202008021305)]
    public class AddFmcpWaybillId : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Shippings", new Column("FmcpWaybillId",  DbType.String.WithSize(50)));
        }
    }
}