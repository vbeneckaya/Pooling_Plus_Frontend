using ThinkingHome.Migrator.Framework;

namespace DAL.Migrations
{
    [Migration(2020_05_03_16_13)]
    public class RenameIncoiceAmountWithoutVatToInvoiceAmmount : Migration
    {
        public override void Apply()
        {
            Database.RenameColumn("Shippings", "InvoiceAmountWithoutVAT", "InvoiceAmount");
        }
    }
}