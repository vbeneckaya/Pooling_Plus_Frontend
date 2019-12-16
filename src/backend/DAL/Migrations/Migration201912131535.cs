using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(201912131535)]
    public class AddCompanyIdToDictionaries : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("DocumentTypes", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("Tonnages", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("BodyTypes", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("VehicleTypes", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("PickingTypes", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("Articles", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("ShippingWarehouses", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("Warehouses", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("Users", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("Roles", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("Tariffs", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
        }
    }
}