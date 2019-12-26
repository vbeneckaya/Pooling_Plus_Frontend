using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(201912180120)]
    public class AddProductTypes : Migration
    {
        public override void Apply()
        {
            Database.AddTable("ProductTypes",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String),
                new Column("PoolingId", DbType.String.WithSize(100)),
                new Column("CompanyId", DbType.Guid, ColumnProperty.Null),
                new Column("IsActive", DbType.Boolean, defaultValue: true));
        }
    }
}
