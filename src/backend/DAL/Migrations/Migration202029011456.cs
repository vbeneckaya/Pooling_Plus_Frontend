using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(202029011456)]
    public class AddUserPoolingAndFmCredentials : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Users", new Column("PoolingLogin", DbType.String.WithSize(50)));
            Database.AddColumn("Users", new Column("PoolingPassword", DbType.String.WithSize(50)));
            Database.AddColumn("Users", new Column("PoolingAccessToken", DbType.String.WithSize(1000)));
            Database.AddColumn("Users", new Column("PoolingRefreshToken", DbType.String.WithSize(1000)));
            Database.AddColumn("Users", new Column("FmCPLogin", DbType.String.WithSize(50)));
            Database.AddColumn("Users", new Column("FmCPPassword", DbType.String.WithSize(50)));
            Database.AddColumn("Users", new Column("FmCPAccessToken", DbType.String.WithSize(1000)));
            Database.AddColumn("Users", new Column("FmCPRefreshToken", DbType.String.WithSize(1000)));
        }
    }
}