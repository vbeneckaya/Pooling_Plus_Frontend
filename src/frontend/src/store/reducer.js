import { combineReducers } from 'redux';

import { connectRouter } from 'connected-react-router';
import { default as profile } from '../ducks/profile';
import { default as login } from '../ducks/login';
import { default as roles } from '../ducks/roles';
import { default as users } from '../ducks/users';

export default history =>
    combineReducers({
        login,
        profile,
        roles,
        users,
        router: connectRouter(history),
    });
