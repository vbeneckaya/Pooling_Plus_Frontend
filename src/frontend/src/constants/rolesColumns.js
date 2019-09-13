import { ACTIVE_TYPE, ENUM_TYPE } from '../constants/columnsType';

export const rolesColumns = [
    {
        name: 'name',
    },
    {
        name: 'users',
    },
    {
        name: 'permissions',
        type: ENUM_TYPE,
    },
    {
        name: 'isActive',
        type: ACTIVE_TYPE,
    },
];
