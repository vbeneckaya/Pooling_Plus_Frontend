using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202002111715)]
    public class RemoveTarifficationTypeFromTariffs : Migration
    {
         public override void Apply()
        {
            Database.RemoveColumn("Tariffs", "TarifficationType");
        }
    }
}