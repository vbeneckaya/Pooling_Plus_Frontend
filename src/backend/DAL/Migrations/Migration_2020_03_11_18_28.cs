using System;
using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(2020_03_11_18_28)]
    public class Migration_2020_03_11_18_28 : Migration
    {
        public override void Apply()
        {
            Database.ExecuteNonQuery(@"
                DELETE FROM ""UserSettings""
                WHERE ""UserId"" IS null AND ""Key"" LIKE 'shippings';"
            );

            InsertDefaultUserSettings("shippings",
                "{\"null\":[{\"source\":\"shippings\",\"showRawValue\":false,\"name\":\"shippingNumber\",\"displayNameKey\":\"shippingNumber\",\"type\":\"Link\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":true},{\"source\":\"shippingState\",\"showRawValue\":false,\"name\":\"status\",\"displayNameKey\":\"Shipping.Status\",\"type\":\"State\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":true},{\"source\":\"transportCompanies\",\"showRawValue\":false,\"name\":\"carrierId\",\"displayNameKey\":\"carrierId\",\"type\":\"Select\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":false},{\"source\":\"providers\",\"showRawValue\":false,\"name\":\"providerId\",\"displayNameKey\":\"providerId\",\"type\":\"Select\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":false},{\"source\":\"shippingPoolingState\",\"showRawValue\":false,\"name\":\"poolingStatus\",\"displayNameKey\":\"Pooling.State\",\"type\":\"State\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":true},{\"name\":\"palletsCount\",\"displayNameKey\":\"palletsCount\",\"type\":\"Number\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":false},{\"name\":\"weightKg\",\"displayNameKey\":\"weightKg\",\"type\":\"Number\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":false},{\"source\":\"vehicleTypes\",\"showRawValue\":false,\"name\":\"vehicleTypeId\",\"displayNameKey\":\"vehicleTypeId\",\"type\":\"Select\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":false},{\"name\":\"totalDeliveryCost\",\"displayNameKey\":\"totalDeliveryCost\",\"type\":\"Number\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":false},{\"source\":\"tarifficationType\",\"showRawValue\":false,\"name\":\"tarifficationType\",\"displayNameKey\":\"tarifficationType\",\"type\":\"Enum\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":false},{\"name\":\"driver\",\"displayNameKey\":\"driver\",\"type\":\"Text\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":false},{\"name\":\"vehicleNumber\",\"displayNameKey\":\"vehicleNumber\",\"type\":\"Text\",\"isDefault\":true,\"isFixedPosition\":false,\"isRequired\":false,\"isReadOnly\":false}]}");
        }

        private void InsertDefaultUserSettings(string key, string value)
        {
            Database.Insert("UserSettings", new[] {"Id", "UserId", "Key", "Value"},
                new[] {Guid.NewGuid().ToString(), null, key, value});
        }
    }
}