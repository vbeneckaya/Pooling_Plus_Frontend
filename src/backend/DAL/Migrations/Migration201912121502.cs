using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(201912121502)]
    public class AddCompanyTable : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Companies",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String.WithSize(255)),
                new Column("IsActive", DbType.Boolean, defaultValue: true));
        }
    }
}
