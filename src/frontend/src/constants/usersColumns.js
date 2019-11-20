import { ACTIVE_TYPE, ENUM_TYPE, SELECT_TYPE } from './columnTypes';

export const usersColumns = [
    {
        name: 'email',
    },
    {
        name: 'userName',
    },
    {
        name: 'role',
        isTranslate: true,
    },
    {
        name: 'carrierId',
        type: SELECT_TYPE,
        source: 'transportCompanies',
    },
    {
        name: 'isActive',
        type: ACTIVE_TYPE,
    },
];
