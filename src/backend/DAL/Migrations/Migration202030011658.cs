using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202030011658)]
    public class ChangeClientIdToProviderIdInShippingWarehousesTable : Migration
    {
        public override void Apply()
        {
            Database.RenameColumn("ShippingWarehouses", "ClientId", "ProviderId");
        }
    }
}