import {ACTIVE_TYPE, ENUM_TYPE} from "../utils/gridColumnsHelper";

export const rolesColumns = [
    {
        key: 'name',
        text: 'Наименование'
    },
    {
        key: 'users',
        text: 'Пользователей'
    },
    {
        key: 'permissions',
        text: 'Разрешения',
        type: ENUM_TYPE
    },
    {
        key: 'is_active',
        text: 'Активен',
        type: ACTIVE_TYPE
    }
];
