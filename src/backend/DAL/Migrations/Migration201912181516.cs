using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(201912181516)]
    public class AddPoolingIdToBodyTypes : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("BodyTypes", new Column("PoolingId", DbType.String, ColumnProperty.Null));
        }
    }
}