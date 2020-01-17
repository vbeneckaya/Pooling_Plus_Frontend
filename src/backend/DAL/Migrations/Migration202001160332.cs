using System;
using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(202001160332)]
    public class AddDriverAngVehicleNumberInOrders : Migration
    {
        public override void Apply()
        {
            Database.AddColumn("Orders", new Column("Driver", DbType.String));
            Database.AddColumn("Orders", new Column("VehicleNumber", DbType.String.WithSize(50)));
            
            AddTranslation("vehicleNumber", "Vehicle Number", "Номер ТС");
            AddTranslation("driver", "Driver", "Водитель");
        }
        
        private string AddTranslation(string name, string en, string ru)
        {
            var id = (Guid.NewGuid()).ToString();
            Database.Insert("Translations", new string[] { "Id", "Name", "En", "Ru" },
                new string[] { id, name, en, ru });
            return id;
        }
    }
}