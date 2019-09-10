using System;
using System.Data;
using Infrastructure.Extensions;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace DAL.Migrations
{
    [Migration(1)]
    public class Migration001 : Migration
    {
        public override void Apply()
        {
            Database.AddTable("Roles",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String.WithSize(100)));
            Database.AddIndex("roles_pk", true, "Roles", "Id");
            
            var administratorRoleId = AddRole("Administrator");
            var transportСoordinatorId = AddRole("TransportCoordinator");
            var transportCompanyEmployee = AddRole("TransportCompanyEmployee");

            Database.AddTable("Users",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String.WithSize(100)),
                new Column("RoleId", DbType.Guid),
                new Column("Email", DbType.String.WithSize(100)),
                new Column("IsActive", DbType.Boolean),
                new Column("FieldsConfig", DbType.String.WithSize(300)),
                new Column("PasswordHash", DbType.String.WithSize(300)));
            
            Database.AddIndex("users_pk", true, "Users", "Id");
            
            AddUser("Иван Иванов", administratorRoleId, "admin@admin.ru", "123".GetHash());
            AddUser("Максим Координатович", transportСoordinatorId, "max@tms.ru", "123".GetHash());
            AddUser("Сергей Газельев", transportCompanyEmployee, "gazelev@tms.ru", "123".GetHash());
            
            Database.AddTable("Translations",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Name", DbType.String.WithSize(100)),
                new Column("En", DbType.String.WithSize(100)),
                new Column("Ru", DbType.String.WithSize(100))
                );
            Database.AddIndex("translations_pk", true, "Translations", "Id");
            
            AddTranslation("UserNotFound", "User not found", "Пользователь не найден или не активен");
            AddTranslation("UserIncorrectData", "The username or password you entered is incorrect", "Неверное имя пользователя или пароль");
            AddTranslation("Users", "Users", "Пользователи");
            AddTranslation("Roles", "Roles", "Роли");
            AddTranslation("Orders", "Orders", "Заказы");
            AddTranslation("Transportations", "Transportations", "Перевозки");
            AddTranslation("Name", "Name", "Имя");
            AddTranslation("IsActive", "IsActive", "Активен");
            AddTranslation("Administrator", "Administrator", "Администратор");
            AddTranslation("TransportCoordinator", "Transport coordinator", "Транспортный координатор");
            AddTranslation("TransportCompanyEmployee", "Transport company employee", "Представитель транспортной компании");
            AddTranslation("Incoming", "Incoming", "Входящий номер");
            
            Database.AddTable("Orders",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("Incoming", DbType.String.WithSize(100))
            );
            Database.AddIndex("orders_pk", true, "Orders", "Id");
            
            AddOrder("56454");
            AddOrder("45745");
            AddOrder("19278");

            Database.AddTable("Transportations",
                new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey),
                new Column("From", DbType.String.WithSize(100)),
                new Column("To", DbType.String.WithSize(100))
            );
            Database.AddIndex("transportations_pk", true, "Transportations", "Id");
            
            AddTransportation("Москва", "Саратов");
            AddTransportation("Москва", "Саратов");
            AddTransportation("Москва", "Саратов");
            AddTransportation("Москва", "Саратов");
        }

        private void AddTransportation(string from, string to)
        {
            Database.Insert("Transportations", new string[] {"Id", "From", "To"},
                    new string[] {(Guid.NewGuid()).ToString(), from, to});
        }

        private void AddOrder(string incoming)
        {
            Database.Insert("Orders", new string[] {"Id", "Incoming"},
                    new string[] {(Guid.NewGuid()).ToString(), incoming});
        }

        private string AddTranslation(string name, string en, string ru)
        {
            var id = (Guid.NewGuid()).ToString();
            Database.Insert("Translations", new string[] {"Id", "Name", "En", "Ru"},
                new string[] {id, name, en, ru});
            return id;
        }

        private string AddRole(string name)
        {
            var id = (Guid.NewGuid()).ToString();
            Database.Insert("Roles", new string[] {"Id", "Name"},
                new string[] {id, name});
            return id;
        }

        private void AddUser(string name, string roleid, string email, string passwordhash)
        {
            Database.Insert("Users", new string[] {"Id", "Name", "RoleId", "Email", "IsActive", "FieldsConfig", "PasswordHash"},
                new string[] {(Guid.NewGuid()).ToString(), name, roleid, email, "true", "", passwordhash});
        }
    }
}