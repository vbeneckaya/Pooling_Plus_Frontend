using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202002121058)]
    public class Migration202002121058 : Migration
    {
        public override void Apply()
        {
            Database.RenameColumn("Shippings", "TarifficationType", );
        }
    }
}