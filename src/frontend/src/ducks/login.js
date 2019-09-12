import { all, takeEvery, delay, put } from 'redux-saga/effects';
import { createSelector } from 'reselect';
import loginPage from '../mocks/loginPage';
import {postman, setAccessToken} from '../utils/postman';
import { push } from 'connected-react-router';
import { getUserProfile } from './profile';

//*  TYPES  *//

const GET_LOGIN_PAGE_REQUEST = 'GET_LOGIN_PAGE_REQUEST';
const GET_LOGIN_PAGE_SUCCESS = 'GET_LOGIN_PAGE_SUCCESS';
const GET_LOGIN_PAGE_ERROR = 'GET_LOGIN_PAGE_ERROR';

const LOGIN_REQUEST = 'LOGIN_REQUEST';
const LOGIN_SUCCESS = 'LOGIN_SUCCESS';
const LOGIN_ERROR = 'LOGIN_ERROR';

setAccessToken && setAccessToken(localStorage.getItem('accessToken'));

//*  INITIAL STATE  *//

const initial = {
    page: {},
    error: '',
    isAuth: Boolean(localStorage.getItem('accessToken')),
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
            return {
                ...state,
                login_progress: true,
                isAuth: false,
                error: '',
            };
        case LOGIN_SUCCESS:
            return {
                ...state,
                login_progress: false,
                isAuth: true,
                error: '',
            };
        case LOGIN_ERROR:
            return {
                ...state,
                login_progress: false,
                isAuth: false,
                error: payload,
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

//*  SELECTORS *//

const stateSelector = state => state.login;
const getKey = (state, key) => key;
export const loginPageSelector = createSelector(
    stateSelector,
    state => state.page,
);
export const progressSelector = createSelector(
    [stateSelector, getKey],
    (state, key) => state[key],
);
export const errorSelector = createSelector(
    stateSelector,
    state => state.error,
);
export const isAuthSelector = createSelector(
    stateSelector,
    state => state.isAuth,
);

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
        yield put({
            type: LOGIN_SUCCESS,
        });
        localStorage.setItem('accessToken', result.accessToken);
        setAccessToken(result.accessToken);
        yield put(push('/'));
        yield put(getUserProfile());
    } catch ({response}) {
        yield put({
            type: LOGIN_ERROR,
            payload: response.data,
        });
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_LOGIN_PAGE_REQUEST, getLoginPageSaga),
        takeEvery(LOGIN_REQUEST, loginSaga),
    ]);
}
