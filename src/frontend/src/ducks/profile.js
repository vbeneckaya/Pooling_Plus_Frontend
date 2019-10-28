import { all, put, takeEvery } from 'redux-saga/effects';
import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import { push as historyPush } from 'connected-react-router';
import {FIELDS_SETTING_LINK, ROLES_LINK, USERS_LINK} from '../router/links';
import { logoutRequest } from './login';

//*  TYPES  *//
export const GET_USER_PROFILE_REQUEST = 'GET_USER_PROFILE_REQUEST';
const GET_USER_PROFILE_SUCCESS = 'GET_USER_PROFILE_SUCCESS';
const GET_USER_PROFILE_ERROR = 'GET_USER_PROFILE_ERROR';

//*  INITIAL STATE  *//

const initial = {
    progress: false,
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_USER_PROFILE_REQUEST:
            return {
                ...state,
                progress: true,
            };
        case GET_USER_PROFILE_SUCCESS:
            return {
                ...state,
                progress: false,
                ...payload,
            };
        case GET_USER_PROFILE_ERROR:
            return {
                ...state,
                progress: false,
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const getUserProfile = payload => {
    return {
        type: GET_USER_PROFILE_REQUEST,
        payload,
    };
};

//*  SELECTORS *//

const stateSelector = state => state.profile;
export const gridsMenuSelector = createSelector(
    stateSelector,
    state => state.grids && state.grids.map(grid => grid.name),
);
export const dictionariesMenuSelector = createSelector(
    stateSelector,
    state =>
        state.dictionaries &&
        state.dictionaries
            .filter(dictionary => !dictionary.showOnHeader)
            .map(dictionary => dictionary.name),
);

export const dictionariesHeaderSelector = createSelector(
    stateSelector,
    state =>
        state.dictionaries &&
        state.dictionaries
            .filter(dictionary => dictionary.showOnHeader)
            .map(dictionary => dictionary.name),
);

export const otherMenuSelector = createSelector(stateSelector, state => {
    const menu = []
   if (true) {
       menu.push({
           name: 'fields_setting',
           link: FIELDS_SETTING_LINK
       })
   }

   return menu;
});

export const userNameSelector = createSelector(
    stateSelector,
    state => state.userName,
);
export const roleSelector = createSelector(
    stateSelector,
    state => state.userRole,
);

export const rolesAndUsersMenu = createSelector(
    stateSelector,
    state => {
        let menu = [];

        if (state.editRoles) {
            menu.push({
                name: 'roles',
                link: ROLES_LINK,
            });
        }

        if (state.editUsers) {
            menu.push({
                name: 'users',
                link: USERS_LINK,
            });
        }

        return menu;
    },
);

export const homePageSelector = createSelector(
    stateSelector,
    state => (state.grids && state.grids.length ? state.grids[0].name : ''),
);

//*  SAGA  *//

function* getUserProfileSaga({ payload = {} }) {
    try {
        const userInfo = yield postman.get('/identity/userInfo');
        const config = yield postman.get('/appConfiguration');
        const { url } = payload;

        yield put({
            type: GET_USER_PROFILE_SUCCESS,
            payload: { ...userInfo, ...config },
        });
        if (url) {
            yield put(historyPush(url));
        }
    } catch (e) {
        yield put({
            type: GET_USER_PROFILE_ERROR,
            payload: e,
        });

        yield put(logoutRequest()); // todo
    }
}

export function* saga() {
    yield all([takeEvery(GET_USER_PROFILE_REQUEST, getUserProfileSaga)]);
}
