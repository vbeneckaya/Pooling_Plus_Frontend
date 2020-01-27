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
        name: 'providerId',
        displayNameKey: 'providerId',
        type: SELECT_TYPE,
        source: 'Providers',
    },
    {
        name: 'clientId',
        displayNameKey: 'clientId',
        type: SELECT_TYPE,
        source: 'Clients',
    },
    {
        name: 'isActive',
        displayNameKey: 'isActive',
        type: ACTIVE_TYPE,
    },
];
