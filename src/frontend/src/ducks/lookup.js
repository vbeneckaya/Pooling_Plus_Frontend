import {all, put, takeEvery} from 'redux-saga/effects';
import {createSelector} from 'reselect';
import {postman} from '../utils/postman';

//*  TYPES  *//

const GET_LOOKUP_REQUEST = 'GET_LOOKUP_REQUEST';
const GET_LOOKUP_SUCCESS = 'GET_LOOKUP_SUCCESS';
const GET_LOOKUP_ERROR = 'GET_LOOKUP_ERROR';

//*  INITIAL STATE  *//

const initial = {
    list: [],
    progress: false
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_LOOKUP_REQUEST:
            return {
              ...state,
              progress: true
            };
        case GET_LOOKUP_SUCCESS:
            return {
                ...state,
                list: payload,
                progress: false
            };
        case GET_LOOKUP_ERROR:
            return {
                ...state,
                progress: false
            };
        default:
            return state;
    }
}

//*  ACTION CREATORS  *//

export const getLookupRequest = payload => {
    return {
        type: GET_LOOKUP_REQUEST,
        payload
    }
};

//*  SELECTORS *//

const stateSelector = state => state.lookup;
export const listSelector = createSelector(stateSelector, state => state.list) || [];
export const progressSelector = createSelector(stateSelector, state => state.progress);

//*  SAGA  *//

function* getLookupSaga ({ payload }) {
    try {
        const {name, params} = payload;
        const result = yield postman.post(`/dictionary/${name}/search`, params);

        yield put({
            type: GET_LOOKUP_SUCCESS,
            payload: result
        })
    } catch (e) {
        yield put({
            type: GET_LOOKUP_ERROR,
            payload: e
        })
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_LOOKUP_REQUEST, getLookupSaga)
    ])
}
