import { all, delay, put, takeEvery } from 'redux-saga/effects';
import { createSelector } from 'reselect';
import loginPage from '../mocks/loginPage';
import { postman, setAccessToken } from '../utils/postman';

import { GET_USER_PROFILE_REQUEST } from './profile';

//*  TYPES  *//

const GET_LOGIN_PAGE_REQUEST = 'GET_LOGIN_PAGE_REQUEST';
const GET_LOGIN_PAGE_SUCCESS = 'GET_LOGIN_PAGE_SUCCESS';
const GET_LOGIN_PAGE_ERROR = 'GET_LOGIN_PAGE_ERROR';

const LOGIN_REQUEST = 'LOGIN_REQUEST';
const LOGIN_SUCCESS = 'LOGIN_SUCCESS';
const LOGIN_ERROR = 'LOGIN_ERROR';

const LOGIN_BY_TOKEN_REQUEST = 'LOGIN_BY_TOKEN_REQUEST';
const LOGIN_BY_TOKEN_SUCCESS = 'LOGIN_BY_TOKEN_SUCCESS';
const LOGIN_BY_TOKEN_ERROR = 'LOGIN_BY_TOKEN_ERROR';

const LOGOUT_REQUEST = 'LOGOUT_REQUEST';

export const ACCESS_TOKEN = 'accessToken';

//*  INITIAL STATE  *//

const initial = {
    page: {},
    error: '',
    isAuth: Boolean(localStorage.getItem(ACCESS_TOKEN)),
    page_progress: false,
    login_progress: false,
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_LOGIN_PAGE_REQUEST:
            return {
                ...state,
                page_progress: true,
            };
        case GET_LOGIN_PAGE_SUCCESS:
            return {
                ...state,
                page: payload,
                page_progress: false,
            };
        case GET_LOGIN_PAGE_ERROR:
            return {
                ...state,
                page: {},
                page_progress: false,
            };
        case LOGIN_REQUEST:
        case LOGIN_BY_TOKEN_REQUEST:
            return {
                ...state,
                login_progress: true,
                isAuth: false,
                error: '',
            };
        case LOGIN_SUCCESS:
        case LOGIN_BY_TOKEN_SUCCESS:
            return {
                ...state,
                login_progress: false,
                isAuth: true,
                error: '',
            };
        case LOGIN_ERROR:
        case LOGIN_BY_TOKEN_ERROR:
            return {
                ...state,
                login_progress: false,
                isAuth: false,
                error: payload,
            };
        case LOGOUT_REQUEST:
            return {
                ...state,
                isAuth: false,
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const getLoginPageRequest = payload => {
    return {
        type: GET_LOGIN_PAGE_REQUEST,
        payload,
    };
};

export const loginRequest = payload => {
    return {
        type: LOGIN_REQUEST,
        payload,
    };
};

export const loginByTokenRequest = payload => {
    return {
        type: LOGIN_BY_TOKEN_REQUEST,
        payload,
    };
};

export const logoutRequest = () => {
    localStorage.removeItem(ACCESS_TOKEN);
    return {
        type: LOGOUT_REQUEST,
    };
};

//*  SELECTORS *//

const stateSelector = state => state.login;
const getKey = (state, key) => key;
export const loginPageSelector = createSelector(stateSelector, state => state.page);
export const progressSelector = createSelector([stateSelector, getKey], (state, key) => state[key]);
export const errorSelector = createSelector(stateSelector, state => state.error);
export const isAuthSelector = createSelector(stateSelector, state => state.isAuth);

//*  SAGA  *//

function* getLoginPageSaga({ payload }) {
    try {
        yield delay(1000);
        const result = loginPage;
        yield put({
            type: GET_LOGIN_PAGE_SUCCESS,
            payload: result,
        });
    } catch (e) {
        yield put({
            type: GET_LOGIN_PAGE_ERROR,
            payload: e,
        });
    }
}

function* loginSaga({ payload }) {
    try {
        yield delay(1000);
        const { api, form } = payload;
        const result = yield postman.post('/identity/login', form);
        localStorage.setItem(ACCESS_TOKEN, result.accessToken);
        setAccessToken(result.accessToken);
        yield put({
            type: LOGIN_SUCCESS,
        });
        yield put({
            type: GET_USER_PROFILE_REQUEST,
            payload: { url: '/' },
        });
    } catch ({ response }) {
        yield put({
            type: LOGIN_ERROR,
            payload: response.data,
        });
    }
}

function* loginByTokenSaga({ payload }) {
    try {
        const {userId, token } = payload;
        const result = yield postman.post(`/identity/loginByToken/`, payload);
        localStorage.setItem(ACCESS_TOKEN, result.accessToken);
        setAccessToken(result.accessToken);
        yield put({
            type: LOGIN_BY_TOKEN_SUCCESS,
        });
        yield put({
            type: GET_USER_PROFILE_REQUEST,
            payload: { url: '/' },
        });
    } catch ({ response }) {
        yield put({
            type: LOGIN_BY_TOKEN_ERROR,
            payload: response.data,
        });
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_LOGIN_PAGE_REQUEST, getLoginPageSaga),
        takeEvery(LOGIN_REQUEST, loginSaga),
        takeEvery(LOGIN_BY_TOKEN_REQUEST, loginByTokenSaga),
    ]);
}
