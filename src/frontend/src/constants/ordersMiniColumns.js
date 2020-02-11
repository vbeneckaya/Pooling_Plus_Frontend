export const ordersMiniColumns = [
    {
        "source": "clients",
        "showRawValue": false,
        "name": "clientId",
        "displayNameKey": "clientId",
        "type": "Select",
        "isDefault": true,
        "isFixedPosition": false,
        "isRequired": true,
        "isReadOnly": false
    },
    {
        "source": "orders",
        "showRawValue": false,
        "name": "orderNumber",
        "displayNameKey": "orderNumber",
        "type": "Link",
        "isDefault": true,
        "isFixedPosition": false,
        "isRequired": true,
        "isReadOnly": true
    },
    {
        "name": "palletsCount",
        "displayNameKey": "palletsCount",
        "type": "Number",
        "isDefault": true,
        "isFixedPosition": false,
        "isRequired": false,
        "isReadOnly": false
    },
    {
        "name": "weightKg",
        "displayNameKey": "weightKg",
        "type": "Number",
        "isDefault": true,
        "isFixedPosition": false,
        "isRequired": false,
        "isReadOnly": false
    },
    {
        "source": "shippingWarehouses",
        "showRawValue": false,
        "name": "shippingWarehouseId",
        "displayNameKey": "shippingWarehouseId",
        "type": "Select",
        "isDefault": true,
        "isFixedPosition": false,
        "isRequired": true,
        "isReadOnly": false
    },
    {
        "source": "warehouses",
        "showRawValue": false,
        "name": "deliveryWarehouseId",
        "displayNameKey": "deliveryWarehouseId",
        "type": "Select",
        "isDefault": true,
        "isFixedPosition": false,
        "isRequired": true,
        "isReadOnly": false
    },
    {
        "name": "shippingDate",
        "displayNameKey": "shippingDate",
        "type": "Date",
        "isDefault": true,
        "isFixedPosition": false,
        "isRequired": false,
        "isReadOnly": false
    },
    {
        "name": "deliveryDate",
        "displayNameKey": "deliveryDate",
        "type": "Date",
        "isDefault": true,
        "isFixedPosition": false,
        "isRequired": true,
        "isReadOnly": false
    }
];