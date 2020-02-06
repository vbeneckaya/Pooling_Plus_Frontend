import {createSelector} from 'reselect';
import {downloader, postman} from '../utils/postman';
import {all, put, takeEvery} from 'redux-saga/effects';
import {ORDERS_GRID} from '../constants/grids';
import {toast} from "react-toastify";
import {SETTINGS_TYPE_HIDE} from "../constants/formTypes";

const TYPE_API = 'fieldProperties';

//*  TYPES  *//

const GET_FIELDS_SETTINGS_REQUEST = 'GET_FIELDS_SETTINGS_REQUEST';
const GET_FIELDS_SETTINGS_SUCCESS = 'GET_FIELDS_SETTINGS_SUCCESS';
const GET_FIELDS_SETTINGS_ERROR = 'GET_FIELDS_SETTINGS_ERROR';

const GET_ORDER_IN_SHIPPING_FIELDS_SETTINGS_REQUEST = 'GET_ORDER_IN_SHIPPING_FIELDS_SETTINGS_REQUEST';
const GET_ORDER_IN_SHIPPING_FIELDS_SETTINGS_SUCCESS = 'GET_ORDER_IN_SHIPPING_FIELDS_SETTINGS_SUCCESS';

const EDIT_FIELDS_SETTINGS_REQUEST = 'EDIT_FIELDS_SETTINGS_REQUEST';
const EDIT_FIELDS_SETTINGS_SUCCESS = 'EDIT_FIELDS_SETTINGS_SUCCESS';
const EDIT_FIELDS_SETTINGS_ERROR = 'EDIT_FIELDS_SETTINGS_ERROR';

const IMPORT_FIELDS_SETTINGS_REQUEST = 'IMPORT_FIELDS_SETTINGS_REQUEST';
const IMPORT_FIELDS_SETTINGS_SUCCESS = 'IMPORT_FIELDS_SETTINGS_SUCCESS';
const IMPORT_FIELDS_SETTINGS_ERROR = 'IMPORT_FIELDS_SETTINGS_ERROR';

const EXPORT_FIELDS_SETTINGS_REQUEST = 'EXPORT_FIELDS_SETTINGS_REQUEST';
const EXPORT_FIELDS_SETTINGS_SUCCESS = 'EXPORT_FIELDS_SETTINGS_SUCCESS';
const EXPORT_FIELDS_SETTINGS_ERROR = 'EXPORT_FIELDS_SETTINGS_ERROR';

const TOGGLE_HIDDEN_STATE_REQUEST = 'TOGGLE_HIDDEN_STATE_REQUEST';
const TOGGLE_HIDDEN_STATE_SUCCESS = 'TOGGLE_HIDDEN_STATE_SUCCESS';
const TOGGLE_HIDDEN_STATE_ERROR = 'TOGGLE_HIDDEN_STATE_ERROR';

const CLEAR_FIELDS_SETTINGS = 'CLEAR_FIELDS_SETTINGS';

//*  INITIAL STATE  *//

const initial = {
    settings: {},
    extSettings: {},
    progress: false,
    editProgress: false,
    importProgress: false,
    exportProgress: false
};

//*  REDUCER  *//

export default (state = initial, {type, payload = {}}) => {
    switch (type) {
        case GET_FIELDS_SETTINGS_REQUEST:
        case GET_ORDER_IN_SHIPPING_FIELDS_SETTINGS_REQUEST:
            return {
                ...state,
                progress: true,
            };
        case GET_FIELDS_SETTINGS_SUCCESS:
            return {
                ...state,
                settings: payload,
                progress: false,
            };
        case GET_ORDER_IN_SHIPPING_FIELDS_SETTINGS_SUCCESS:
            return {
                ...state,
                extSettings: payload,
                progress: false,
            };
        case EDIT_FIELDS_SETTINGS_REQUEST:
            const {params = {}} = payload;

            return {
                ...state,
                editProgress: {
                    field: params.fieldName,
                    state: params.state,
                },
            };
        case EDIT_FIELDS_SETTINGS_SUCCESS:
        case EDIT_FIELDS_SETTINGS_ERROR:
            return {
                ...state,
                editProgress: false,
            };
        case GET_FIELDS_SETTINGS_ERROR:
            return {
                ...state,
                settings: {},
                extSettings: {},
                progress: false,
            };
        case CLEAR_FIELDS_SETTINGS:
            return {
                ...state,
                ...initial,
            };

        case IMPORT_FIELDS_SETTINGS_REQUEST:
            return {
                ...state,
                importProgress: true
            };
        case IMPORT_FIELDS_SETTINGS_SUCCESS:
        case IMPORT_FIELDS_SETTINGS_ERROR:
            return {
                ...state,
                importProgress: false,
            };

        case EXPORT_FIELDS_SETTINGS_REQUEST:
            return {
                ...state,
                exportProgress: true
            };
        case EXPORT_FIELDS_SETTINGS_SUCCESS:
        case EXPORT_FIELDS_SETTINGS_ERROR:
            return {
                ...state,
                exportProgress: false,
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const getFieldsSettingRequest = payload => {
    return {
        type: GET_FIELDS_SETTINGS_REQUEST,
        payload,
    };
};

export const editFieldsSettingRequest = payload => {
    return {
        type: EDIT_FIELDS_SETTINGS_REQUEST,
        payload,
    };
};

export const importFieldsSettingRequest = payload => {
    return {
        type: IMPORT_FIELDS_SETTINGS_REQUEST,
        payload,
    };
};

export const exportFieldsSettingRequest = payload => {
    return {
        type: EXPORT_FIELDS_SETTINGS_REQUEST,
        payload,
    };
};

export const clearFieldsSettings = () => {
    return {
        type: CLEAR_FIELDS_SETTINGS,
    };
};

export const toggleHidenStateRequest = payload => {
    return {
        type: TOGGLE_HIDDEN_STATE_REQUEST,
        payload,
    };
};

//*  SELECTORS *//

const stateSelector = state => state.fieldsSetting;

export const fieldsSettingSelector = createSelector(stateSelector, state => state.settings);

export const fieldsExtSettingSelector = createSelector(stateSelector, state => state.extSettings);

export const progressSelector = createSelector(stateSelector, state => state.progress);

export const editProgressSelector = createSelector(stateSelector, state => state.editProgress);

export const importProgressSelector = createSelector(stateSelector, state => state.importProgress);
export const exportProgressSelector = createSelector(stateSelector, state => state.exportProgress);

//*  SAGA  *//
export function* getFieldsSettingSaga({payload}) {
    try {
        const baseResult = yield postman.post(`${TYPE_API}/get`, payload);
        const extResult = yield postman.post(`${TYPE_API}/get`, {
            ...payload,
            forEntity: payload.forEntity === ORDERS_GRID ? 'orderItems' : 'routePoints',
        });

        yield put({
            type: GET_FIELDS_SETTINGS_SUCCESS,
            payload: {
                base: baseResult,
                ext: extResult,
            },
        });
    } catch (e) {
        yield put({
            type: GET_FIELDS_SETTINGS_ERROR,
            payload: e,
        });
    }
}

export function* getShippingOrderFieldsSettingSaga({payload}) {
    try {
        const baseResult = yield postman.post(`${TYPE_API}/get`, payload);
        const extResult = yield postman.post(`${TYPE_API}/get`, {
            ...payload,
            forEntity: 'orderItems'
        });

        yield put({
            type: GET_ORDER_IN_SHIPPING_FIELDS_SETTINGS_SUCCESS,
            payload: {
                base: baseResult,
                ext: extResult,
            },
        });
    } catch (e) {
        yield put({
            type: GET_FIELDS_SETTINGS_ERROR,
            payload: e,
        });
    }
}


function* editFieldsSettingSaga({payload = {}}) {
    try {
        const {params, callbackSuccess, isExt} = payload;
        const result = yield postman.post(`/${TYPE_API}/save`, {
            ...params,
            forEntity: isExt
                ? params.forEntity === ORDERS_GRID
                    ? 'orderItems'
                    : 'routePoints'
                : params.forEntity,
        });

        yield put({
            type: EDIT_FIELDS_SETTINGS_SUCCESS,
        });

        callbackSuccess && callbackSuccess();
    } catch (e) {
        yield put({
            type: EDIT_FIELDS_SETTINGS_ERROR,
            payload: e,
        });
    }
}

export function* exportFieldsSettingSaga({payload}) {
    try {
        const res = yield downloader.post(`${TYPE_API}/export/${payload.forEntity}/${payload.forRoleId}`, payload.fieldProperties.base, {
            responseType: 'blob',
        });
        const {data} = res;
        let headerLine = res.headers['content-disposition'];
        let startFileNameIndex = headerLine.indexOf('filename=') + 10;
        let endFileNameIndex = headerLine.lastIndexOf(';') - 1;
        let filename = headerLine.substring(startFileNameIndex, endFileNameIndex);

        const link = document.createElement('a');
        link.href = URL.createObjectURL(new Blob([data], {type: data.type}));
        link.setAttribute('download', filename);
        document.body.appendChild(link);
        link.click();
        yield put({type: EXPORT_FIELDS_SETTINGS_SUCCESS});
    } catch (e) {
        yield put({
            type: EXPORT_FIELDS_SETTINGS_ERROR,
            payload: e,
        });
    }
}

export function* importFieldsSettingSaga({payload}) {
    try {
        const {entity, role, form, callbackSuccess} = payload;
        const result = yield postman.post(`${TYPE_API}/import/${entity}/${role}`, form, {
            headers: {'Content-Type': 'multipart/form-data'},
        });
        toast.info("ОК ");
        yield put({
            type: IMPORT_FIELDS_SETTINGS_SUCCESS,
        });
        callbackSuccess();
    } catch (e) {
        yield put({
            type: IMPORT_FIELDS_SETTINGS_ERROR,
        });
    }
}

function* toggleHiddenStateSaga({payload}) {
    try {
        const {params, callbackSuccess, isExt} = payload;
        const result = yield postman.post(`/${TYPE_API}/toggleHiddenState`, {
            ...params,
            forEntity: isExt
                ? params.forEntity === ORDERS_GRID
                    ? 'orderItems'
                    : 'routePoints'
                : params.forEntity,
        });

        yield put({
            type: TOGGLE_HIDDEN_STATE_SUCCESS,
        });

        callbackSuccess && callbackSuccess();
    } catch (e) {
        yield put({
            type: TOGGLE_HIDDEN_STATE_ERROR,
            payload: e,
        });
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_FIELDS_SETTINGS_REQUEST, getFieldsSettingSaga),
        takeEvery(GET_ORDER_IN_SHIPPING_FIELDS_SETTINGS_REQUEST, getShippingOrderFieldsSettingSaga),
        takeEvery(EDIT_FIELDS_SETTINGS_REQUEST, editFieldsSettingSaga),
        takeEvery(TOGGLE_HIDDEN_STATE_REQUEST, toggleHiddenStateSaga),
        takeEvery(IMPORT_FIELDS_SETTINGS_REQUEST, importFieldsSettingSaga),
        takeEvery(EXPORT_FIELDS_SETTINGS_REQUEST, exportFieldsSettingSaga),
    ]);
}
