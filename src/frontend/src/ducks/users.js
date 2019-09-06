import { all, put, takeEvery, delay } from 'redux-saga/effects';
import { postman } from '../utils/postman';
import { createSelector } from 'reselect';
import { toast } from 'react-toastify';
import users from "../mocks/users";

const TYPE_API = 'users';

//*  TYPES  *//

const GET_USERS_LIST_REQUEST = 'GET_USERS_LIST_REQUEST';
const GET_USERS_LIST_SUCCESS = 'GET_USERS_LIST_SUCCESS';
const GET_USERS_LIST_ERROR = 'GET_USERS_LIST_ERROR';

const GET_USER_CARD_REQUEST = 'GET_USER_CARD_REQUEST';
const GET_USER_CARD_SUCCESS = 'GET_USER_CARD_SUCCESS';
const GET_USER_CARD_ERROR = 'GET_USER_CARD_ERROR';

//*  INITIAL STATE  *//

const initial = {
    list: [],
    card: {},
    totalCount: 0,
    progress: false,
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_USERS_LIST_REQUEST:
        case GET_USER_CARD_REQUEST:
            return {
                ...state,
                progress: true,
                error: ''
            };
        case GET_USERS_LIST_SUCCESS:
            return {
                ...state,
                progress: false,
                error: '',
                list: payload.isConcat ? [...state.list, ...payload.users] : payload.users,
                totalCount: payload.total_count
            };
        case GET_USER_CARD_SUCCESS:
            return {
                ...state,
                progress: false,
                card: payload
            };
        case GET_USERS_LIST_ERROR:
        case GET_USER_CARD_ERROR:
            return {
                ...state,
                progress: false,
                error: payload
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const getUsersRequest = payload => {
    return {
        type: GET_USERS_LIST_REQUEST,
        payload,
    };
};

export const getUserCardRequest = payload => {
    return {
        type: GET_USER_CARD_REQUEST,
        payload
    }
};


//*  SELECTORS *//

const stateSelector = state => state.users;
export const usersListSelector = createSelector(stateSelector, state => state.list);
export const progressSelector = createSelector(stateSelector, state => state.progress);
export const totalCountSelector = createSelector(stateSelector, state => state.totalCount);
export const userCardSelector = createSelector(stateSelector, state => state.card);

//*  SAGA  *//

function* getUsersListSaga({ payload }) {
    try {
        yield delay(2000);
        const { filter = {}, isConcat } = payload;
        const result = users;

        yield put({
            type: GET_USERS_LIST_SUCCESS,
            payload: {
                ...result,
                isConcat
            },
        });
    } catch (error) {
        yield put({
            type: GET_USERS_LIST_ERROR,
            payload: error,
        });
        toast.error('Ошибка!');
    }
}

function* getUserCardSaga({ payload }) {
    try {
        yield delay(2000);
        const result = users.users.find(item => item.id === payload);

        yield put({
            type: GET_USER_CARD_SUCCESS,
            payload: result,
        });
    } catch (error) {
        yield put({
            type: GET_USER_CARD_ERROR,
            payload: error,
        });
        toast.error('Ошибка!');
    }
}


export function* saga() {
    yield all([
        takeEvery(GET_USERS_LIST_REQUEST, getUsersListSaga),
        takeEvery(GET_USER_CARD_REQUEST, getUserCardSaga),
    ]);
}
