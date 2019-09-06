import { all, put, takeEvery, delay } from 'redux-saga/effects';
import { postman } from '../utils/postman';
import { createSelector } from 'reselect';
import { toast } from 'react-toastify';
import roles from '../mocks/roles';

const TYPE_API = 'roles';

//*  TYPES  *//

const GET_ROLES_LIST_REQUEST = 'GET_ROLES_LIST_REQUEST';
const GET_ROLES_LIST_SUCCESS = 'GET_ROLES_LIST_SUCCESS';
const GET_ROLES_LIST_ERROR = 'GET_ROLES_LIST_ERROR';

const GET_ROLE_CARD_REQUEST = 'GET_ROLE_CARD_REQUEST';
const GET_ROLE_CARD_SUCCESS = 'GET_ROLE_CARD_SUCCESS';
const GET_ROLE_CARD_ERROR = 'GET_ROLE_CARD_ERROR';

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
        case GET_ROLES_LIST_REQUEST:
        case GET_ROLE_CARD_REQUEST:
            return {
                ...state,
                progress: true,
                error: '',
            };
        case GET_ROLES_LIST_SUCCESS:
            return {
                ...state,
                progress: false,
                error: '',
                list: payload.isConcat ? [...state.list, ...payload.roles] : payload.roles,
                totalCount: payload.total_count,
            };
        case GET_ROLE_CARD_SUCCESS:
            return {
                ...state,
                progress: false,
                card: payload,
            };
        case GET_ROLES_LIST_ERROR:
        case GET_ROLE_CARD_ERROR:
            return {
                ...state,
                progress: false,
                error: payload,
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const getRolesRequest = payload => {
    return {
        type: GET_ROLES_LIST_REQUEST,
        payload,
    };
};

export const getRoleCardRequest = payload => {
    return {
        type: GET_ROLE_CARD_REQUEST,
        payload,
    };
};

//*  SELECTORS *//

const stateSelector = state => state.roles;
export const rolesListSelector = createSelector(stateSelector, state => state.list);
export const progressSelector = createSelector(stateSelector, state => state.progress);
export const totalCountSelector = createSelector(stateSelector, state => state.totalCount);
export const roleCardSelector = createSelector(stateSelector, state => state.card)

//*  SAGA  *//

function* getRolesListSaga({ payload }) {
    try {
        yield delay(2000);
        const { filter = {}, isConcat } = payload;
        const result = roles;

        yield put({
            type: GET_ROLES_LIST_SUCCESS,
            payload: {
                ...result,
                isConcat,
            },
        });
    } catch (error) {
        yield put({
            type: GET_ROLES_LIST_ERROR,
            payload: error,
        });
        toast.error('Ошибка!');
    }
}


function* getRoleCardSaga({ payload }) {
    try {
        yield delay(2000);
        const result = roles.roles.find(item => item.id === payload);

        yield put({
            type: GET_ROLE_CARD_SUCCESS,
            payload: result,
        });
    } catch (error) {
        yield put({
            type: GET_ROLE_CARD_ERROR,
            payload: error,
        });
        toast.error('Ошибка!');
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_ROLES_LIST_REQUEST, getRolesListSaga),
        takeEvery(GET_ROLE_CARD_REQUEST, getRoleCardSaga),
    ]);
}
