using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202010021511)]
    public class AddUserCreatorIdToShipping : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Shippings", new Column("UserCreatorId",  DbType.Guid, ColumnProperty.Null));
        }
    }
}