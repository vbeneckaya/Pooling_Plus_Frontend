import {ACTIVE_TYPE, ENUM_TYPE, LABELS_TYPE, SELECT_TYPE} from './columnTypes';

export const rolesColumns = [
    {
        name: 'name',
        displayNameKey: 'name',
    },
    {
        name: 'usersCount',
        displayNameKey: 'usersCount',
    },
    {
        name: 'permissions',
        displayNameKey: 'permissions',
        type: LABELS_TYPE,
    },
    {
        name: 'isActive',
        displayNameKey: 'isActive',
        type: ACTIVE_TYPE,
    },
];
