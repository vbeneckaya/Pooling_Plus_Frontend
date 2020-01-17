import { all, put, takeEvery, take, spawn } from 'redux-saga/effects';
import { createSelector } from 'reselect';
import { postman } from '../utils/postman';

//*  TYPES  *//
const GET_REPORT_REQUEST = 'GET_REPORT_REQUEST';
const GET_REPORT_SUCCESS = 'GET_REPORT_SUCCESS';
const GET_REPORT_ERROR = 'GET_REPORT_ERROR';

//*  INITIAL STATE  *//

const initial = {
    data: {},
    progress: false,
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_REPORT_REQUEST:
            return {
                ...state,
                progress: true,
            };
        case GET_REPORT_SUCCESS:
            return {
                ...state,
                progress: false,
                data: payload,
            };
        case GET_REPORT_ERROR:
            return {
                ...state,
                progress: false,
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const getReportRequest = payload => {
    return {
        type: GET_REPORT_REQUEST,
        payload
    }
};

//*  SELECTORS *//
const stateSelector = state => state.reports;
export const reportSelector = createSelector(stateSelector, state => state.data);
export const reportProgressSelector = createSelector(stateSelector, state => state.progress);

//*  SAGA  *//

function* getReportSaga({ payload }) {
    try {
        const result = yield postman.get('/report/config');

        yield put({
            type: GET_REPORT_SUCCESS,
            payload: result
        })
    } catch (e) {
        yield put({
            type: GET_REPORT_ERROR
        })
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_REPORT_REQUEST, getReportSaga)
    ])
}
