import { all, takeEvery, delay, put } from 'redux-saga/effects';
import { createSelector } from 'reselect';
import loginPage from '../mocks/loginPage';
import {postman} from "../utils/postman";
import { push } from 'connected-react-router'

//*  TYPES  *//

const GET_LOGIN_PAGE_REQUEST = 'GET_LOGIN_PAGE_REQUEST';
const GET_LOGIN_PAGE_SUCCESS = 'GET_LOGIN_PAGE_SUCCESS';
const GET_LOGIN_PAGE_ERROR = 'GET_LOGIN_PAGE_ERROR';

const LOGIN_REQUEST = 'LOGIN_REQUEST';
const LOGIN_SUCCESS = 'LOGIN_SUCCESS';
const LOGIN_ERROR = 'LOGIN_ERROR';

//*  INITIAL STATE  *//

const initial = {
    page: {},
    error: '',
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
                error: '',
            };
        case LOGIN_SUCCESS:
            return {
                ...state,
                login_progress: false,
                error: '',
            };
        case LOGIN_ERROR:
            return {
                ...state,
                login_progress: false,
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
export const loginPageSelector = createSelector(stateSelector, state => state.page);
export const progressSelector = createSelector([stateSelector, getKey], (state, key) => state[key]);
export const errorSelector = createSelector(stateSelector, state => state.error);

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
        const result = {}// yield postman[api.type](api.url, form);
        yield put({
            type: LOGIN_SUCCESS,
            payload: result,
        });
        yield put(push('/'))
    } catch (e) {
        yield put({
            type: LOGIN_ERROR,
            payload: 'Ошибка!',
        });
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_LOGIN_PAGE_REQUEST, getLoginPageSaga),
        takeEvery(LOGIN_REQUEST, loginSaga),
    ]);
}
