using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(201910211254)]
    public class CreateRolePermissionsTable : Migration
    {
        public override void Apply()
        {
            Database.AddTable("RolePermissions",
               new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
               new Column("RoleId", DbType.Guid),
               new Column("PermissionCode", DbType.Int32)
               );
            
            Database.AddIndex("RolePermissions_Roles_fk", false, "RolePermissions", "RoleId");

            if (Database.ConstraintExists("RolePermissions", "RolePermissions_Constr"))
            {
                Database.RemoveConstraint("RolePermissions", "RolePermissions_Constr");
            }

            Database.AddUniqueConstraint("RolePermissions_Constr", "RolePermissions", "RoleId", "PermissionCode");
        }
    }
}
