using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(201912161523)]
    public class AddClients : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Clients",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String),
                new Column("PoolingId", DbType.String),
                new Column("CompanyId", DbType.Guid, ColumnProperty.Null),
                new Column("IsActive", DbType.Boolean, defaultValue: true));
        }
    }
}
