import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import { all, takeEvery, put, cancelled, delay, fork, cancel } from 'redux-saga/effects';

//*  TYPES  *//

const GET_ACTIONS_REQUEST = 'GET_ACTIONS_REQUEST';
const GET_ACTIONS_SUCCESS = 'GET_ACTIONS_SUCCESS';
const GET_ACTIONS_ERROR = 'GET_ACTIONS_ERROR';

//*  INITIAL STATE  *//

const initial = {
    actions: []
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
                actions: payload
            };
        case GET_ACTIONS_ERROR:
            return {
                ...state,
            };
        default:
            return state;
    }
}

//*  ACTION CREATORS  *//

export const getActionsRequest = payload => {
    return {
        type: GET_ACTIONS_REQUEST,
        payload
    }
};

//*  SELECTORS *//

const stateSelector = state => state.gridActions;

export const actionsSelector = createSelector(stateSelector, state => state.actions);

//*  SAGA  *//

function* getActionsSaga ({ payload }) {
    try {
        const {name, ids} = payload;
        const result = yield postman.post(`/${name}/getActions`, ids);

        yield put({
            type: GET_ACTIONS_SUCCESS,
            payload: result
        })
    } catch (e) {
        yield put({type: GET_ACTIONS_ERROR})
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_ACTIONS_REQUEST, getActionsSaga)
    ])
}
