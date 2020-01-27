using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202023011051)]
    public class RemoveCompaniesTable : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Users", new Column("ClientId", DbType.Guid));
            
            Database.RemoveColumn("Users", "CompanyId");
            Database.RemoveColumn("BodyTypes", "CompanyId");
            Database.RemoveColumn("Clients", "CompanyId");
            Database.RemoveColumn("DocumentTypes", "CompanyId");
            Database.RemoveColumn("FieldPropertyItems", "CompanyId");
            Database.RemoveColumn("FieldPropertyVisibilityItems", "CompanyId");
            Database.RemoveColumn("Orders", "CompanyId");
            Database.RemoveColumn("PickingTypes", "CompanyId");
            Database.RemoveColumn("ProductTypes", "CompanyId");
            Database.RemoveColumn("Roles", "CompanyId");
            Database.RemoveColumn("ShippingWarehouses", "CompanyId");
            Database.AddColumn("ShippingWarehouses",  new Column("ClientId", DbType.Guid));
            Database.RemoveColumn("Warehouses", "CompanyId");
            Database.AddColumn("Warehouses", new Column("ProviderId", DbType.Guid));
            Database.RemoveColumn("Shippings", "CompanyId");
            Database.RemoveColumn("Tonnages", "CompanyId");
            Database.RemoveColumn("TransportCompanies", "CompanyId");
            Database.RemoveColumn("VehicleTypes", "CompanyId");
            Database.RemoveTable("Companies");
        }

    }
}
