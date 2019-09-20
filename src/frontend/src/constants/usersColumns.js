import { ACTIVE_TYPE, ENUM_TYPE } from './columnTypes';

export const usersColumns = [
    {
        name: 'login'
    },
    {
        name: 'name',
    },
    {
        name: 'email',
    },
    {
        name: 'role',
    },
    {
        name: 'isActive',
        type: ACTIVE_TYPE,
    },
];
