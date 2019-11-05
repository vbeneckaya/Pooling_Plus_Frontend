import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import { all, takeEvery, put, cancelled, delay, fork, cancel, select } from 'redux-saga/effects';
import { ORDERS_GRID } from '../constants/grids';
//*  TYPES  *//

const GET_FIELDS_SETTINGS_REQUEST = 'GET_FIELDS_SETTINGS_REQUEST';
const GET_FIELDS_SETTINGS_SUCCESS = 'GET_FIELDS_SETTINGS_SUCCESS';
const GET_FIELDS_SETTINGS_ERROR = 'GET_FIELDS_SETTINGS_ERROR';

const EDIT_FIELDS_SETTINGS_REQUEST = 'EDIT_FIELDS_SETTINGS_REQUEST';
const EDIT_FIELDS_SETTINGS_SUCCESS = 'EDIT_FIELDS_SETTINGS_SUCCESS';
const EDIT_FIELDS_SETTINGS_ERROR = 'EDIT_FIELDS_SETTINGS_ERROR';

const CLEAR_FIELDS_SETTINGS = 'CLEAR_FIELDS_SETTINGS';

//*  INITIAL STATE  *//

const initial = {
    settings: {},
    progress: false,
    editProgress: false,
};

//*  REDUCER  *//

export default (state = initial, { type, payload = {} }) => {
    switch (type) {
        case GET_FIELDS_SETTINGS_REQUEST:
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
        case EDIT_FIELDS_SETTINGS_REQUEST:
            const { params = {} } = payload;

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
                progress: false,
            };
        case CLEAR_FIELDS_SETTINGS:
            return {
                ...state,
                ...initial
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

export const clearFieldsSettings = () => {
    return {
        type: CLEAR_FIELDS_SETTINGS
    }
};

//*  SELECTORS *//

const stateSelector = state => state.fieldsSetting;

export const fieldsSettingSelector = createSelector(stateSelector, state => state.settings);

export const progressSelector = createSelector(stateSelector, state => state.progress);

export const editProgressSelector = createSelector(stateSelector, state => state.editProgress);

//*  SAGA  *//
export function* getFieldsSettingSaga({payload}) {
    try {
        const baseResult = yield postman.post('fieldProperties/get', payload);
        const extResult =
            payload.forEntity === ORDERS_GRID
                ? yield postman.post('fieldProperties/get', {
                      ...payload,
                      forEntity: 'orderItems',
                  })
                : [];

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

function* editFieldsSettingSaga({ payload }) {
    try {
        const { params, callbackSuccess, isExt } = payload;
        const result = yield postman.post('/fieldProperties/save', {
            ...params,
            forEntity: isExt ? 'orderItems' : params.forEntity
        });

        yield put({
            type: EDIT_FIELDS_SETTINGS_REQUEST,
        });

        callbackSuccess && callbackSuccess();
    } catch (e) {
        yield put({
            type: EDIT_FIELDS_SETTINGS_ERROR,
            payload: e,
        });
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_FIELDS_SETTINGS_REQUEST, getFieldsSettingSaga),
        takeEvery(EDIT_FIELDS_SETTINGS_REQUEST, editFieldsSettingSaga),
    ]);
}
