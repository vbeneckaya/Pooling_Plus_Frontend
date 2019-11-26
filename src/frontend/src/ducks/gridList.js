import { createSelector } from 'reselect';
import { downloader, postman } from '../utils/postman';
import { all, cancel, cancelled, delay, fork, put, select, takeEvery } from 'redux-saga/effects';
import { IS_AUTO_UPDATE } from '../constants/settings';
import { formatDate } from '../utils/dateTimeFormater';
import { toast } from 'react-toastify';
import { representationFromGridSelector } from './representations';

let task = null;
let filters = {};

//*  TYPES  *//

const GET_GRID_LIST_REQUEST = 'GET_GRID_LIST_REQUEST';
const GET_GRID_LIST_SUCCESS = 'GET_GRID_LIST_SUCCESS';
const GET_GRID_LIST_ERROR = 'GET_GRID_LIST_ERROR';

const GET_STATE_COLORS_REQUEST = 'GET_STATE_COLORS_REQUEST';
const GET_STATE_COLORS_SUCCESS = 'GET_STATE_COLORS_SUCCESS';
const GET_STATE_COLORS_ERROR = 'GET_STATE_COLORS_ERROR';

const GRID_IMPORT_FROM_EXCEL_REQUEST = 'GRID_IMPORT_FROM_EXCEL_REQUEST';
const GRID_IMPORT_FROM_EXCEL_SUCCESS = 'GRID_IMPORT_FROM_EXCEL_SUCCESS';
const GRID_IMPORT_FROM_EXCEL_ERROR = 'GRID_IMPORT_FROM_EXCEL_ERROR';

const GRID_EXPORT_TO_EXCEL_REQUEST = 'GRID_EXPORT_TO_EXCEL_REQUEST';
const GRID_EXPORT_TO_EXCEL_SUCCESS = 'GRID_EXPORT_TO_EXCEL_SUCCESS';
const GRID_EXPORT_TO_EXCEL_ERROR = 'GRID_EXPORT_TO_EXCEL_ERROR';

const GRID_AUTO_UPDATE_START = 'GRID_AUTO_UPDATE_START';
const GRID_AUTO_UPDATE_STOP = 'GRID_AUTO_UPDATE_STOP';

const CLEAR_GRID_INFO = 'CLEAR_GRID_INFO';

//*  INITIAL STATE  *//

const initial = {
    data: [],
    totalCount: 0,
    stateColors: [],
    progress: false,
    stateColorsProgress: false,
    importProgress: false,
    exportProgress: false,
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_GRID_LIST_REQUEST:
            return {
                ...state,
                progress: !payload.notLoader,
            };
        case GET_GRID_LIST_SUCCESS:
            return {
                ...state,
                progress: false,
                totalCount: payload.totalCount,
                data: payload.isConcat ? [...state.data, ...payload.items] : payload.items,
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
                stateColorsProgress: true,
            };
        case GET_STATE_COLORS_SUCCESS:
            return {
                ...state,
                stateColors: payload,
                stateColorsProgress: false,
            };
        case GET_STATE_COLORS_ERROR:
            return {
                ...state,
                stateColors: [],
                stateColorsProgress: false,
            };
        case GRID_IMPORT_FROM_EXCEL_REQUEST:
            return {
                ...state,
                importProgress: true,
            };
        case GRID_IMPORT_FROM_EXCEL_SUCCESS:
        case GRID_IMPORT_FROM_EXCEL_ERROR:
            return {
                ...state,
                importProgress: false,
            };
        case GRID_EXPORT_TO_EXCEL_REQUEST:
            return {
                ...state,
                exportProgress: true,
            };
        case GRID_EXPORT_TO_EXCEL_SUCCESS:
        case GRID_EXPORT_TO_EXCEL_ERROR:
            return {
                ...state,
                exportProgress: false,
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
        payload,
    };
};

export const autoUpdateStart = payload => {
    return { type: GRID_AUTO_UPDATE_START, payload };
};

export const autoUpdateStop = payload => {
    return { type: GRID_AUTO_UPDATE_STOP, payload };
};

export const importFromExcelRequest = payload => {
    return {
        type: GRID_IMPORT_FROM_EXCEL_REQUEST,
        payload,
    };
};

export const exportToExcelRequest = payload => {
    return {
        type: GRID_EXPORT_TO_EXCEL_REQUEST,
        payload,
    };
};

export const clearGridInfo = () => {
    return {
        type: CLEAR_GRID_INFO,
    };
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

export const stateColorsSelector = createSelector(
    stateSelector,
    state => state.stateColors,
);

export const canCreateByFormSelector = createSelector(
    [stateProfile, gridName],
    (state, name) => {
        const grid = state.grids && state.grids.find(item => item.name === name);
        return grid ? grid.canCreateByForm : false;
    },
);

export const canImportFromExcelSelector = createSelector(
    [stateProfile, gridName],
    (state, name) => {
        const grid = state.grids && state.grids.find(item => item.name === name);
        return grid ? grid.canImportFromExcel : false;
    },
);

export const canExportToExcelSelector = createSelector(
    [stateProfile, gridName],
    (state, name) => {
        const grid = state.grids && state.grids.find(item => item.name === name);
        return grid ? grid.canExportToExcel : false;
    },
);

export const importProgressSelector = createSelector(
    stateSelector,
    state => state.importProgress,
);

export const exportProgressSelector = createSelector(
    stateSelector,
    state => state.exportProgress,
);

//*  SAGA  *//

export function* getListSaga({ payload }) {
    try {
        const { filter = {}, name, isConcat } = payload;

        const representation = yield select(state => representationFromGridSelector(state, name));
        const columns = representation ? representation.map(item => item.name) : [];

        const result = yield postman.post(`/${name}/search`, {
            ...filter,
            filter: {
                ...filter.filter,
                columns,
            },
        });

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
                notLoader: true,
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

export function* autoUpdateStopSaga({ payload = {} }) {
    if (task) {
        const { isClear } = payload;
        if (isClear) yield put(clearGridInfo());
        yield cancel(task);
        task = null;
        filters = {};
    }
}

export const backgroundSyncListSaga = function*() {
    try {
        while (true) {
            yield put(getListRequest(filters));
            yield delay(30000);
        }
    } finally {
        if (yield cancelled()) {
            console.log('---', 'cancelled sync saga');
        }
    }
};

function* getStateColorsSaga({ payload }) {
    try {
        const result = yield postman.post(`${payload}/search`);

        yield put({
            type: GET_STATE_COLORS_SUCCESS,
            payload: result,
        });
    } catch (e) {
        yield put({
            type: GET_STATE_COLORS_ERROR,
            payload: e,
        });
    }
}

function* importFromExcelSaga({ payload }) {
    try {
        const { form, name, callbackSuccess } = payload;
        const result = yield postman.post(`${name}/importFromExcel`, form, {
            headers: { 'Content-Type': 'multipart/form-data' },
        });

        if (result.isError) {
            toast.error(result.error);
        } else {
            yield put({
                type: GRID_IMPORT_FROM_EXCEL_SUCCESS,
            });

            callbackSuccess();
        }
    } catch (e) {
        yield put({
            type: GRID_IMPORT_FROM_EXCEL_ERROR,
        });
    }
}

function* exportToExcelSaga({ payload }) {
    try {
        const { name, filter } = payload;
        const representation = yield select(state => representationFromGridSelector(state, name));
        const columns = representation ? representation.map(item => item.name) : [];
        /*const fileName = `${name}_${formatDate(new Date(), 'YYYY-MM-dd_HH_mm_ss')}.xlsx`;
       const result = yield postman.post(
            `/${name}/exportToExcel`,
            { columns, filter },
            { responseType: 'blob' },
        );
        const link = document.createElement('a');
        link.href = URL.createObjectURL(new Blob([result], { type: result.type }));
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();*/

        const res = yield downloader.post(
            `/${name}/exportToExcel`,
            { columns, ...filter },
            { responseType: 'blob' },
        );
        const { data } = res;
        let headerLine = res.headers['content-disposition'];
        let startFileNameIndex = headerLine.indexOf('filename=') + 10;
        let endFileNameIndex = headerLine.lastIndexOf(';') - 1;
        let filename = headerLine.substring(startFileNameIndex, endFileNameIndex);

        const link = document.createElement('a');
        link.href = URL.createObjectURL(new Blob([data], { type: data.type }));
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
        yield put({ type: GRID_EXPORT_TO_EXCEL_SUCCESS });
    } catch (error) {
        yield put({ type: GRID_EXPORT_TO_EXCEL_ERROR });
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_GRID_LIST_REQUEST, getListSaga),
        takeEvery(GRID_AUTO_UPDATE_START, autoUpdateStartSaga),
        takeEvery(GRID_AUTO_UPDATE_STOP, autoUpdateStopSaga),
        takeEvery(GET_STATE_COLORS_REQUEST, getStateColorsSaga),
        takeEvery(GRID_IMPORT_FROM_EXCEL_REQUEST, importFromExcelSaga),
        takeEvery(GRID_EXPORT_TO_EXCEL_REQUEST, exportToExcelSaga),
    ]);
}
