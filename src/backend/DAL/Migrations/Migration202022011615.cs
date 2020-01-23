using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(202022011615)]
    public class AddProvidersTable : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Providers",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String.WithSize(200)),
                new Column("Inn", DbType.String.WithSize(12)),
                new Column("Cpp", DbType.String.WithSize(10)),
                new Column("LegalAddress", DbType.String.WithSize(250)),
                new Column("ActualAddress", DbType.String.WithSize(250)),
                new Column("ContactPerson", DbType.String.WithSize(300)),
                new Column("ContactPhone", DbType.String.WithSize(16)),
                new Column("Email", DbType.String.WithSize(70)),
                new Column("IsActive", DbType.Boolean, defaultValue: true)
            );

            Database.AddColumn("Users", new Column("ProviderId", DbType.Guid));
            Database.AddColumn("Tariffs", new Column("ProviderId", DbType.Guid));
            Database.RemoveColumn("Tariffs", "CompanyId");
        }
    }
}