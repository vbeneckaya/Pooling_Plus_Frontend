import { ACTIVE_TYPE, ENUM_TYPE, SELECT_TYPE } from './columnTypes';

export const usersColumns = [
    {
        name: 'email',
        displayNameKey: 'email',
    },
    {
        name: 'userName',
        displayNameKey: 'userName',
    },
    {
        name: 'role',
        displayNameKey: 'role',
        isTranslate: true,
    },
    {
        name: 'carrierId',
        displayNameKey: 'carrierId',
        type: SELECT_TYPE,
        source: 'transportCompanies',
    },
    {
        name: 'companyId',
        displayNameKey: 'companyId',
        source: 'companies',
        type: SELECT_TYPE,
    },
    {
        name: 'isActive',
        displayNameKey: 'isActive',
        type: ACTIVE_TYPE,
    },
];
