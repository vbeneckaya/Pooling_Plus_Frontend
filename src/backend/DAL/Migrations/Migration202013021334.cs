using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202013021334)]
    public class AddUserCreatorIdToOrder : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Orders", new Column("UserCreatorId",  DbType.Guid, ColumnProperty.Null));
        }
    }
}