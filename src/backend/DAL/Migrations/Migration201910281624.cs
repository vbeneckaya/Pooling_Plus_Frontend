﻿using System.Data;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(201910281624)]
    public class RemoveAdminPermissionRolePermissions : Migration
    {
        public override void Apply()
        {
            Database.ExecuteNonQuery(@"
                UPDATE public.""Roles""
                SET ""Permissions"" = '{1, 2, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23}'
                WHERE ""Name"" = 'Administrator';");
        }
    }
}
