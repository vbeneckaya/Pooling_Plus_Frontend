using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(2020_03_10_11_150)]
    public class Migration_2020_03_10_11_50 : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Warehouses", new Column("Longitude", DbType.String));
            Database.AddColumn("Warehouses", new Column("Latitude", DbType.String));
        }
    }
}