import { ACTIVE_TYPE, ENUM_TYPE } from '../constants/columnsType';

export const usersColumns = [
    {
        key: 'login',
        text: 'Логин',
    },
    {
        key: 'name',
        text: 'ФИО',
    },
    {
        key: 'email',
        text: 'Email',
    },
    {
        key: 'role',
        text: 'Роль',
    },
    {
        key: 'shippers',
        text: 'Поставщики',
        type: ENUM_TYPE,
    },
    {
        key: 'regions',
        text: 'Регионы',
        type: ENUM_TYPE,
    },
    {
        key: 'psg',
        text: 'ПСГ',
        type: ENUM_TYPE,
    },
    {
        key: 'is_active',
        text: 'Активность',
        type: ACTIVE_TYPE,
    },
];
