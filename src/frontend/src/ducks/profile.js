import {all, put, takeEvery, take, spawn} from 'redux-saga/effects';
import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import { push as historyPush } from 'connected-react-router';
import { FIELDS_SETTING_LINK, ROLES_LINK, USERS_LINK } from '../router/links';
import { logoutRequest } from './login';
import {clearDictionaryInfo} from './dictionaryView';
import result from "../components/SuperGrid/components/result";

//*  TYPES  *//
export const GET_USER_PROFILE_REQUEST = 'GET_USER_PROFILE_REQUEST';
const GET_USER_PROFILE_SUCCESS = 'GET_USER_PROFILE_SUCCESS';
const GET_USER_PROFILE_ERROR = 'GET_USER_PROFILE_ERROR';

const GET_PROFILE_SETTINGS_REQUEST = 'GET_PROFILE_SETTINGS_REQUEST';
const GET_PROFILE_SETTINGS_SUCCESS = 'GET_PROFILE_SETTINGS_SUCCESS';
const GET_PROFILE_SETTINGS_ERROR = 'GET_PROFILE_SETTINGS_ERROR';

//*  INITIAL STATE  *//

const initial = {
    progress: false,
    data: {},
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
        case GET_PROFILE_SETTINGS_SUCCESS:
            return {
                ...state,
                data: payload
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

export const getProfileSettingsRequest = payload => {
    return {
        type: GET_PROFILE_SETTINGS_REQUEST,
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

export const dictionariesSelector = createSelector(
    stateSelector,
    state => state.dictionaries && state.dictionaries.map(dictionary => dictionary.name),
);

export const isCustomPageSelector = createSelector(stateSelector, state => {
    return {
        editFieldProperties: state.editFieldProperties,
        editRoles: state.editRoles,
        editUsers: state.editUsers,
    };
});

export const otherMenuSelector = createSelector(stateSelector, state => {
    const menu = [];
    if (state.editFieldProperties) {
        menu.push({
            name: 'fields_setting',
            link: FIELDS_SETTING_LINK,
        });
    }

    return menu;
});

export const userNameSelector = createSelector(stateSelector, state => state.userName);
export const roleSelector = createSelector(stateSelector, state => state.userRole);
export const roleIdSelector = createSelector(stateSelector, state => state.role && state.role.id);

export const rolesAndUsersMenu = createSelector(stateSelector, state => {
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
});

export const homePageSelector = createSelector(stateSelector, state => {
    let homePage = '/grid';
    if (state.grids && state.grids.length) {
        homePage = `/grid/${state.grids[0].name}`;
    } else if (state.dictionaries && state.dictionaries.length) {
        homePage = `/dictionary/${state.dictionaries[0].name}`;
    } else if (state.editRoles) {
        homePage = '/roles';
    } else if (state.editUsers) {
        homePage = '/users';
    }

    return homePage;
});

export const userPermissionsSelector = createSelector(stateSelector, state => {
    return state.role ? state.role.permissions.map(item => item.code) : [];
});

export const profileSettingsSelector = createSelector(stateSelector, state => state.data);

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

function* getProfileSettingsSaga() {
    try {
        const result = yield postman.get('/profile');

        yield put({
            type: GET_PROFILE_SETTINGS_SUCCESS,
            payload: result,
        });
    } catch (e) {
        yield put({
            type: GET_PROFILE_SETTINGS_ERROR,
        });
    }
}

function* changeLocation() {
    while (true) {
        const {payload} = yield take('@@router/LOCATION_CHANGE');
        const {location} = payload;
        const {pathname} = location;

        if (pathname.includes('dictionary')) {
            yield put(clearDictionaryInfo());
        }
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_USER_PROFILE_REQUEST, getUserProfileSaga),
        takeEvery(GET_PROFILE_SETTINGS_REQUEST, getProfileSettingsSaga)
    ]);
    yield spawn(changeLocation);
}
