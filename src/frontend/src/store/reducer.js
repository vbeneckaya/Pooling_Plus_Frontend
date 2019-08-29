import { combineReducers } from 'redux';

import { connectRouter } from 'connected-react-router';
import { default as login } from '../ducks/login';

export default history =>
    combineReducers({
        login,
        router: connectRouter(history),
    });
