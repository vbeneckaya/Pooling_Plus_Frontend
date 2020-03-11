using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(2020_03_11_10_56)]
    public class Migrations_2020_03_11_10_56 : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Users", new Column("PasswordToken", DbType.String));
        }
        
    }
}
