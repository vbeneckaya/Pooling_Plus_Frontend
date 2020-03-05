using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202005031116)]
    public class AddPoolingIntegrationFieldInProviders : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Providers", new Column("IsPoolingIntegrated", DbType.Boolean, ColumnProperty.None, false));
        }
    }
}