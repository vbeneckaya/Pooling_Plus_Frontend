using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202002030544)]
    public class AddRouteColumnInShippingsTable : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Shippings", new Column("Route", DbType.String));
        }
    }
}