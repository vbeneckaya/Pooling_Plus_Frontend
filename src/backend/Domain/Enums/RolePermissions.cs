using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum RolePermissions
    {
        // Reserved value
        None = 0,

        OrdersView = 1,
        OrdersCreate = 2,
        //OrdersEditAndDelete = 3,
        OrdersViewAndAttachDocument = 4,
        OrdersEditAndDeleteDocument = 5,
        OrdersViewHistory = 6,

        ShippingsView = 7,
        //ShippingsCreate = 8,
        //ShippingsEditAndDelete = 9,
        ShippingsViewAndAttachDocument = 10,
        ShippingsEditAndDeleteDocument = 11,
        ShippingsViewHistory = 12,

        TariffsView = 13,
        TariffsEdit = 14,

        WarehousesEdit = 15,
        ArticlesEdit = 16,
        PickingTypesEdit = 17,
        TransportCompaniesEdit = 18,
        VehicleTypesEdit = 19,
        DocumentTypesEdit = 20,

        RolesEdit = 21,
        UsersEdit = 22,
        FieldsSettings = 23
    }
}
