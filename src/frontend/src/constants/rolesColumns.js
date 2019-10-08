import {ACTIVE_TYPE, ENUM_TYPE, LABELS_TYPE} from './columnTypes';

export const rolesColumns = [
    {
        name: 'name',
    },
    {
        name: 'users',
    },
    {
        name: 'permissions',
        type: LABELS_TYPE,
    },
    {
        name: 'isActive',
        type: ACTIVE_TYPE,
    },
];
