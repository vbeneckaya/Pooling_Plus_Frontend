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

const CREATE_ROLE_REQUEST = 'CREATE_ROLE_REQUEST';
const CREATE_ROLE_SUCCESS = 'CREATE_ROLE_SUCCESS';
const CREATE_ROLE_ERROR = 'CREATE_ROLE_ERROR';

const CLEAR_ROLES_INFO = 'CLEAR_ROLES_INFO';

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
        case CREATE_ROLE_REQUEST:
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
                list: payload.isConcat ? [...state.list, ...payload.items] : payload.items,
                totalCount: payload.total_count,
            };
        case GET_ROLE_CARD_SUCCESS:
            return {
                ...state,
                progress: false,
                card: payload,
            };
        case CREATE_ROLE_SUCCESS:
            return {
                ...state,
                progress: false,
            };
        case GET_ROLES_LIST_ERROR:
        case GET_ROLE_CARD_ERROR:
        case CREATE_ROLE_ERROR:
            return {
                ...state,
                progress: false,
                error: payload,
            };
        case CLEAR_ROLES_INFO:
            return {
                ...state,
                ...initial,
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

export const createRoleRequest = payload => {
    return {
        type: CREATE_ROLE_REQUEST,
        payload,
    };
};

export const clearRolesInfo = () => {
    return {
        type: CLEAR_ROLES_INFO,
    };
};

//*  SELECTORS *//

const stateSelector = state => state.roles;

export const rolesListSelector = createSelector(
    stateSelector,
    state => state.list,
);

export const rolesFromUserSelector = createSelector(stateSelector, state => {
    console.log('list', state);
    return state.list && state.list.map(item => ({
        name: item.name,
        value: item.id,
        isActive: true
    }))
});


export const progressSelector = createSelector(
    stateSelector,
    state => state.progress,
);
export const totalCountSelector = createSelector(
    stateSelector,
    state => state.totalCount,
);
export const roleCardSelector = createSelector(
    stateSelector,
    state => state.card,
);

//*  SAGA  *//

function* getRolesListSaga({ payload }) {
    try {
        const { filter = {}, isConcat } = payload;
        const result = yield postman.post('/roles/search', filter);

        yield put({
            type: GET_ROLES_LIST_SUCCESS,
            payload: {
                items: result,
                isConcat,
            },
        });
    } catch (error) {
        yield put({
            type: GET_ROLES_LIST_ERROR,
            payload: error,
        });
    }
}

function* getRoleCardSaga({ payload }) {
    try {
        const result = yield postman.get(`/roles/getById/${payload}`);

        yield put({
            type: GET_ROLE_CARD_SUCCESS,
            payload: result,
        });
    } catch (error) {
        yield put({
            type: GET_ROLE_CARD_ERROR,
            payload: error,
        });
    }
}

function* createRoleSaga({ payload }) {
    try {
        const { params, callbackFunc } = payload;

        const result = yield postman.post('/roles/saveOrCreate', params);

        yield put({
            type: CREATE_ROLE_SUCCESS,
        });
        callbackFunc();
    } catch (error) {
        yield put({
            type: CREATE_ROLE_ERROR,
        });
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_ROLES_LIST_REQUEST, getRolesListSaga),
        takeEvery(GET_ROLE_CARD_REQUEST, getRoleCardSaga),
        takeEvery(CREATE_ROLE_REQUEST, createRoleSaga),
    ]);
}
