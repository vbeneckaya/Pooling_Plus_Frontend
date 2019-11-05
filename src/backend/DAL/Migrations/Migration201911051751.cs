using System;
using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(201911051751)]
    public class AddInjectionMessageTranslation : Migration
    {
        public override void Apply()
        {
            AddTranslation("orderCreatedFromInjection", "Order {0} created from file {1}", "Заказ {0} создан из файла {1}");
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
