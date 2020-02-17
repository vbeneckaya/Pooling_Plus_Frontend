using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(202024011022)]
    public class AddFieldsToClientsAndTransportCompaniesTable : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Clients", new Column("Inn", DbType.String.WithSize(12)));
            Database.AddColumn("Clients", new Column("Cpp", DbType.String.WithSize(10)));
            Database.AddColumn("Clients", new Column("LegalAddress", DbType.String.WithSize(250)));
            Database.AddColumn("Clients", new Column("ActualAddress", DbType.String.WithSize(250)));
            Database.AddColumn("Clients", new Column("ContactPerson", DbType.String.WithSize(300)));
            Database.AddColumn("Clients", new Column("ContactPhone", DbType.String.WithSize(16)));
            Database.AddColumn("Clients", new Column("Email", DbType.String.WithSize(70)));

            Database.AddColumn("TransportCompanies", new Column("Inn", DbType.String.WithSize(12)));
            Database.AddColumn("TransportCompanies", new Column("Cpp", DbType.String.WithSize(10)));
            Database.AddColumn("TransportCompanies", new Column("LegalAddress", DbType.String.WithSize(250)));
            Database.AddColumn("TransportCompanies", new Column("ActualAddress", DbType.String.WithSize(250)));
            Database.AddColumn("TransportCompanies", new Column("ContactPerson", DbType.String.WithSize(300)));
            Database.AddColumn("TransportCompanies", new Column("ContactPhone", DbType.String.WithSize(16)));
            Database.AddColumn("TransportCompanies", new Column("Email", DbType.String.WithSize(70)));
        }
    }
}