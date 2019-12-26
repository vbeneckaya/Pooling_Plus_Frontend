using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(201912181745)]
    public class AlterTransportCompanies : Migration
    {
        public override void Apply()
        {
            Database.RemoveColumn("TransportCompanies", "ContractNumber");
            Database.RemoveColumn("TransportCompanies", "DateOfPowerOfAttorney");

            Database.AddColumn("TransportCompanies", new Column("CompanyId", DbType.Guid, ColumnProperty.Null));
            Database.AddColumn("TransportCompanies", new Column("PoolingId", DbType.String, ColumnProperty.Null));
        }
    }
}