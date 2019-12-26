using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(201912170557)]
    public class ChangeOrderClientColumn : Migration
    {
        public override void Apply()
        {
            Database.RemoveColumn("Orders", "ClientName");
            Database.RemoveColumn("Orders", "SoldTo");
            Database.AddColumn("Orders", new Column("ClientId", DbType.Guid, ColumnProperty.Null));
        }
    }
}
