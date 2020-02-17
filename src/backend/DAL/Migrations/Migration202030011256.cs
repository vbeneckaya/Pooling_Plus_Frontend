using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
        [Migration(202030011256)]
        public class ChangeContactPhoneLengthTable : Migration
        {
            public override void Apply()
            {
                Database.ChangeColumn("Providers", "ContactPhone", DbType.String.WithSize(50), false);
                Database.ChangeColumn("Clients", "ContactPhone", DbType.String.WithSize(50), false);
                Database.ChangeColumn("TransportCompanies", "ContactPhone", DbType.String.WithSize(50), false);
            }
        }
}