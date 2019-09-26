import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import { all, takeEvery, put, cancelled, delay, fork, cancel } from 'redux-saga/effects';
import { IS_AUTO_UPDATE } from '../constants/settings';

let task = null;
let filters = {};

//*  TYPES  *//

const GET_GRID_LIST_REQUEST = 'GET_GRID_LIST_REQUEST';
const GET_GRID_LIST_SUCCESS = 'GET_GRID_LIST_SUCCESS';
const GET_GRID_LIST_ERROR = 'GET_GRID_LIST_ERROR';

const GET_STATE_COLORS_REQUEST = 'GET_STATE_COLORS_REQUEST';
const GET_STATE_COLORS_SUCCESS = 'GET_STATE_COLORS_SUCCESS';
const GET_STATE_COLORS_ERROR = 'GET_STATE_COLORS_ERROR';

const GRID_AUTO_UPDATE_START = 'ROUTES_AUTO_UPDATE_START';
const GRID_AUTO_UPDATE_STOP = 'ROUTES_AUTO_UPDATE_STOP';

const CLEAR_GRID_INFO = 'CLEAR_GRID_INFO';

//*  INITIAL STATE  *//

const initial = {
    data: [],
    totalCount: 0,
    stateColors: [],
    progress: false,
    stateColorsProgress: false
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_GRID_LIST_REQUEST:
            return {
                ...state,
                progress: true,
            };
        case GET_GRID_LIST_SUCCESS:
            return {
                ...state,
                progress: false,
                totalCount: payload.totalCount,
                data: payload.isConcat ? [...state.list, ...payload.items] : payload.items,
            };
        case GET_GRID_LIST_ERROR:
            return {
                ...state,
                data: [],
                totalCount: 0,
                progress: false,
            };
        case GET_STATE_COLORS_REQUEST:
            return {
                ...state,
                stateColorsProgress: true
            };
        case GET_STATE_COLORS_SUCCESS:
            return {
                ...state,
                stateColors: payload,
                stateColorsProgress: false
            };
        case GET_STATE_COLORS_ERROR:
            return {
                ...state,
                stateColors: [],
                stateColorsProgress: false
            };
        case CLEAR_GRID_INFO:
            return {
                ...state,
                ...initial,
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const getListRequest = payload => {
    return {
        type: GET_GRID_LIST_REQUEST,
        payload,
    };
};

export const getStateColorsRequest = payload => {
    return {
        type: GET_STATE_COLORS_REQUEST,
        payload
    }
};

export const autoUpdateStart = payload => {
    return { type: GRID_AUTO_UPDATE_START, payload };
};

export const autoUpdateStop = () => {
    return { type: GRID_AUTO_UPDATE_STOP };
};

export const clearGridInfo = () => {
    return {
        type: CLEAR_GRID_INFO
    }
};

//*  SELECTORS *//

const stateSelector = state => state.gridList;
const getKey = (state, key = 'progress') => key;
const stateProfile = state => state.profile;
const gridName = (state, name) => name;

export const columnsGridSelector = createSelector(
    [stateProfile, gridName],
    (state, name) => {
        const grid = state.grids && state.grids.find(item => item.name === name);
        return grid ? grid.columns : [];
    },
);
export const progressSelector = createSelector(
    stateSelector,
    state => state.progress,
);
export const totalCountSelector = createSelector(
    stateSelector,
    state => state.totalCount,
);
export const listSelector = createSelector(
    stateSelector,
    state => state.data,
);

export const stateColorsSelector = createSelector(stateSelector, state => state.stateColors);

export const canCreateByFormSelector = createSelector([stateProfile, gridName], (state, name) => {
    const grid = state.grids && state.grids.find(item => item.name === name);
    return grid ? grid.canCreateByForm : false
});

export const canImportFromExcelSelector = createSelector([stateProfile, gridName], (state, name) => {
    const grid = state.grids && state.grids.find(item => item.name === name);
    return grid ? grid.canImportFromExcel : false
});

//*  SAGA  *//

export function* getListSaga({ payload }) {
    try {
        const { filter = {}, name, isConcat } = payload;

        yield delay(1000);

        const result = yield postman.post(`/${name}/search`, filter);

        yield put({ type: GET_GRID_LIST_SUCCESS, payload: { ...result, isConcat } });
    } catch (error) {
        yield put({ type: GET_GRID_LIST_ERROR, payload: error });
    }
}

export function* autoUpdateStartSaga({ payload }) {
    if (IS_AUTO_UPDATE) {
        if (!task) {
            filters = { ...payload };
            task = yield fork(backgroundSyncListSaga);

            filters = {
                ...filters,
                isConcat: false,
                filter: {
                    ...filters.filter,
                    take: payload.filter.take + payload.filter.skip,
                    skip: 0,
                },
            };
        }
    } else {
        yield put(getListRequest(payload));
    }
}

export function* autoUpdateStopSaga() {
    if (task) {
        yield cancel(task);
        task = null;
        filters = {};
    }
}

export const backgroundSyncListSaga = function*() {
    try {
        while (true) {
            yield put(getListRequest(filters));
            yield delay(60000);
        }
    } finally {
        if (yield cancelled()) {
            console.log('---', 'cancelled sync saga');
        }
    }
};

function* getStateColorsSaga ({payload}) {
    try {
        const result = yield postman.post(`${payload}/search`);

        yield put({
            type: GET_STATE_COLORS_SUCCESS,
            payload: result
        })
    } catch (e) {
        yield put({
            type: GET_STATE_COLORS_ERROR,
            payload: e
        })
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_GRID_LIST_REQUEST, getListSaga),
        takeEvery(GRID_AUTO_UPDATE_START, autoUpdateStartSaga),
        takeEvery(GRID_AUTO_UPDATE_STOP, autoUpdateStopSaga),
        takeEvery(GET_STATE_COLORS_REQUEST, getStateColorsSaga),
    ]);
}
