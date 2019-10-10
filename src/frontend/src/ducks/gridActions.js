import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import { all, takeEvery, put, cancelled, delay, fork, cancel } from 'redux-saga/effects';
import { toast } from 'react-toastify';

//*  TYPES  *//

const GET_ACTIONS_REQUEST = 'GET_ACTIONS_REQUEST';
const GET_ACTIONS_SUCCESS = 'GET_ACTIONS_SUCCESS';
const GET_ACTIONS_ERROR = 'GET_ACTIONS_ERROR';

const INVOKE_ACTION_REQUEST = 'INVOKE_ACTION_REQUEST';
const INVOKE_ACTION_SUCCESS = 'INVOKE_ACTION_SUCCESS';
const INVOKE_ACTION_ERROR = 'INVOKE_ACTION_ERROR';

const CLEAR_ACTIONS = 'CLEAR_ACTIONS';

//*  INITIAL STATE  *//

const initial = {
    actions: [],
    progressActionName: null,
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_ACTIONS_REQUEST:
            return {
                ...state,
            };
        case GET_ACTIONS_SUCCESS:
            return {
                ...state,
                actions: payload,
            };
        case GET_ACTIONS_ERROR:
            return {
                ...state,
            };
        case INVOKE_ACTION_REQUEST:
            return {
                ...state,
                progressActionName: payload.actionName,
            };
        case INVOKE_ACTION_SUCCESS:
            return {
                ...state,
                progressActionName: null,
            };
        case INVOKE_ACTION_ERROR:
            return {
                ...state,
                progressActionName: null,
            };
        case CLEAR_ACTIONS:
            return {
                ...state,
                actions: []
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const getActionsRequest = payload => {
    return {
        type: GET_ACTIONS_REQUEST,
        payload,
    };
};

export const invokeActionRequest = payload => {
    return {
        type: INVOKE_ACTION_REQUEST,
        payload,
    };
};

export const clearActions = () => {
    return {
        type: CLEAR_ACTIONS,
    };
};

//*  SELECTORS *//

const stateSelector = state => state.gridActions;

export const actionsSelector = createSelector(stateSelector, state =>
    state.actions.map(item => ({
        ...item,
        ids: item.ids || [],
    })),
);

export const progressActionNameSelector = createSelector(
    stateSelector,
    state => state.progressActionName,
);

//*  SAGA  *//

function* getActionsSaga({ payload }) {
    try {
        const { name, ids } = payload;
        const result = yield postman.post(`/${name}/getActions`, ids);

        yield put({
            type: GET_ACTIONS_SUCCESS,
            payload: result,
        });
    } catch (e) {
        yield put({ type: GET_ACTIONS_ERROR });
    }
}

function* invokeActionSaga({ payload }) {
    try {
        const { ids, callbackSuccess, name, actionName } = payload;
        const result = yield postman.post(`/${name}/invokeAction/${actionName}`, ids);
        yield put({
            type: INVOKE_ACTION_SUCCESS,
        });
        toast.info(result.message);
        callbackSuccess();
    } catch (e) {
        yield put({
            type: INVOKE_ACTION_ERROR,
            payload: e,
        });
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_ACTIONS_REQUEST, getActionsSaga),
        takeEvery(INVOKE_ACTION_REQUEST, invokeActionSaga),
    ]);
}
