import { ACTIVE_TYPE, ENUM_TYPE } from './columnTypes';

export const usersColumns = [
    {
        name: 'email',
    },
    {
        name: 'name',
    },
    {
        name: 'role',
        isTranslate: true
    },
    {
        name: 'isActive',
        type: ACTIVE_TYPE,
    },
];
