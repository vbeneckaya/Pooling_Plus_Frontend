export const ordersMiniColumns = [
    {
        "source": "clients",
        "showRawValue": false,
        "name": "clientId",
        "displayNameKey": "clientId",
        "type": "Select",
        "isFixedPosition": false,
        "isReadOnly": false
    },
       {
        "source": "orders",
        "showRawValue": false,
        "name": "orderNumber",
        "displayNameKey": "orderNumber",
        "type": "Link",
        "isFixedPosition": false,
        "isReadOnly": false
    },
    {
        "name": "clientOrderNumber",
        "displayNameKey": "clientOrderNumber",
        "type": "Text",
        "isFixedPosition": false,
        "isReadOnly": false
    },
    {
        "name": "palletsCount",
        "displayNameKey": "palletsCount",
        "type": "Number",
        "isFixedPosition": false,
        "isReadOnly": false
    },
    {
        "name": "weightKg",
        "displayNameKey": "weightKg",
        "type": "Number",
        "isFixedPosition": false,
        "isReadOnly": false
    },
    {
        "source": "shippingWarehouses",
        "showRawValue": false,
        "name": "shippingWarehouseId",
        "displayNameKey": "shippingWarehouseId",
        "type": "Select",
        "isFixedPosition": false,
        "isReadOnly": false
    },
    {
        "source": "warehouses",
        "showRawValue": false,
        "name": "deliveryWarehouseId",
        "displayNameKey": "deliveryWarehouseId",
        "type": "Select",
        "isFixedPosition": false,
        "isReadOnly": false
    },
    {
        "name": "shippingDate",
        "displayNameKey": "shippingDate",
        "type": "Date",
        "isDefault": true,
        "isFixedPosition": false,
        "isReadOnly": false
    },
    {
        "name": "deliveryDate",
        "displayNameKey": "deliveryDate",
        "type": "Date",
        "isDefault": true,
        "isFixedPosition": false,
        "isReadOnly": false
    }
];