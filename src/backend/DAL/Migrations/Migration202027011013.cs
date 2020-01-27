using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202027011013)]
    public class AddRoleTypeToRolesTable : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Roles", new Column("RoleType", DbType.Int32, ColumnProperty.NotNull, defaultValue:0));
        }
    }
}