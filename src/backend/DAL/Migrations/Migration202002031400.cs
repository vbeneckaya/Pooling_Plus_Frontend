using System;
using System.Collections.Generic;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(202002031400)]
    public class AddColedinoShippingWarehouse : Migration
    {
        public override void Apply()
        {
            var providersIds = new List<string>();
            var query = Database.FormatSql("SELECT \"Id\" FROM \"Providers\"");
            var reader = Database.ExecuteReader(query);
            while (reader.Read())
            {
                providersIds.Add(reader[0].ToString());
            }

            reader.Close();

            foreach (var providerId in providersIds)
            {
                query =
                    Database.FormatSql($"SELECT \"Id\" FROM \"ShippingWarehouses\" " +
                                       $"WHERE \"ProviderId\" = '{providerId}' AND \"WarehouseName\" = 'Коледино'");
                reader = Database.ExecuteReader(query);
                if (!reader.Read())
                {
                    reader.Close();
                    AddShippingWarehouse(providerId);
                }
                else
                {
                    reader.Close();
                }
            }
        }

        private string AddShippingWarehouse(string providerId)
        {
            var id = Guid.NewGuid().ToString();

            Database.Insert("ShippingWarehouses", new[]
                {
                    "Id", "WarehouseName", "Address", "PostalCode", "Region", "Area", "City",
                    "Street", "House", "IsActive", "Gln", "Settlement", "Block", "RegionId", "UnparsedParts",
                    "ProviderId"
                },
                new[]
                {
                    id,
                    "Коледино",
                    "Московская обл., Подольский р-он с/п Лаговское, вблизи д. Коледино 142181",
                    "142181",
                    "Московская обл.",
                    "Подольский р-он",
                    null,
                    null,
                    null,
                    true.ToString(),
                    null,
                    "с/п Лаговское, вблизи д. Коледино",
                    null,
                    null,
                    null,
                    providerId
                });
            return id;
        }
    }
}