import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import { all, takeEvery, put, cancelled, delay, fork, cancel } from 'redux-saga/effects';

//*  TYPES  *//

const GET_DICTIONARY_LIST_REQUEST = 'GET_DICTIONARY_LIST_REQUEST';
const GET_DICTIONARY_LIST_SUCCESS = 'GET_DICTIONARY_LIST_SUCCESS';
const GET_DICTIONARY_LIST_ERROR = 'GET_DICTIONARY_LIST_ERROR';

const GET_DICTIONARY_CARD_REQUEST = 'GET_DICTIONARY_CARD_REQUEST';
const GET_DICTIONARY_CARD_SUCCESS = 'GET_DICTIONARY_CARD_SUCCESS';
const GET_DICTIONARY_CARD_ERROR = 'GET_DICTIONARY_CARD_ERROR';

const DICTIONARY_IMPORT_FROM_EXCEL_REQUEST = 'DICTIONARY_IMPORT_FROM_EXCEL_REQUEST';
const DICTIONARY_IMPORT_FROM_EXCEL_SUCCESS = 'DICTIONARY_IMPORT_FROM_EXCEL_SUCCESS';
const DICTIONARY_IMPORT_FROM_EXCEL_ERROR = 'DICTIONARY_IMPORT_FROM_EXCEL_ERROR';

const SAVE_DICTIONARY_CARD_REQUEST = 'SAVE_DICTIONARY_CARD_REQUEST';
const SAVE_DICTIONARY_CARD_SUCCESS = 'SAVE_DICTIONARY_CARD_SUCCESS';
const SAVE_DICTIONARY_CARD_ERROR = 'SAVE_DICTIONARY_CARD_ERROR';

const CLEAR_DICTIONARY_INFO = 'CLEAR_DICTIONARY_INFO';

//*  INITIAL STATE  *//

const initial = {
    list: [],
    card: {},
    totalCount: 0,
    progress: false,
    importProgress: false
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_DICTIONARY_LIST_REQUEST:
        case GET_DICTIONARY_CARD_REQUEST:
        case SAVE_DICTIONARY_CARD_REQUEST:
            return {
                ...state,
                progress: true,
            };
        case GET_DICTIONARY_LIST_SUCCESS:
            return {
                ...state,
                list: payload.isConcat ? [...state.list, ...payload.items] : payload.items,
                progress: false,
                totalCount: payload.totalCount,
            };
        case GET_DICTIONARY_LIST_ERROR:
            return {
                ...state,
                progress: false,
                list: [],
                totalCount: 0,
            };
        case GET_DICTIONARY_CARD_SUCCESS:
            return {
                ...state,
                card: payload,
            };
        case GET_DICTIONARY_CARD_ERROR:
            return {
                ...state,
                card: {},
                progress: false,
            };
        case CLEAR_DICTIONARY_INFO:
            return {
                ...state,
                ...initial
            };
        case SAVE_DICTIONARY_CARD_SUCCESS:
        case SAVE_DICTIONARY_CARD_ERROR:
            return {
                ...state,
                progress: false
            };
        case DICTIONARY_IMPORT_FROM_EXCEL_REQUEST:
            return {
                ...state,
                importProgress: true
            };
        case DICTIONARY_IMPORT_FROM_EXCEL_SUCCESS:
        case DICTIONARY_IMPORT_FROM_EXCEL_ERROR:
            return {
                ...state,
                importProgress: false
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const getListRequest = payload => {
    return {
        type: GET_DICTIONARY_LIST_REQUEST,
        payload,
    };
};

export const getCardRequest = payload => {
    return {
        type: GET_DICTIONARY_CARD_REQUEST,
        payload,
    };
};

export const saveDictionaryCardRequest = payload => {
    return {
        type: SAVE_DICTIONARY_CARD_REQUEST,
        payload
    }
};

export const clearDictionaryInfo = () => {
    return {
        type: CLEAR_DICTIONARY_INFO
    }
};

export const importFromExcelRequest = payload => {
    return {
        type: DICTIONARY_IMPORT_FROM_EXCEL_REQUEST,
        payload
    }
};

//*  SELECTORS *//

const stateSelector = state => state.dictionaryView;
const getKey = (state, key = 'progress') => key;
const stateProfile = state => state.profile;
const dictionaryName = (state, name) => name;

export const columnsSelector = createSelector(
    [stateProfile, dictionaryName],
    (state, name) => {
        const dictionary = state.dictionaries && state.dictionaries.find(item => item.name === name);
        return dictionary ? dictionary.columns : [];
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
    state => state.list,
);
export const cardSelector = createSelector(
    stateSelector,
    state => state.card,
);

export const canCreateByFormSelector = createSelector([stateProfile, dictionaryName], (state, name) => {
    const dictionary = state.dictionaries && state.dictionaries.find(item => item.name === name);
    return dictionary ? dictionary.canCreateByForm : false
});

export const canImportFromExcelSelector = createSelector([stateProfile, dictionaryName], (state, name) => {
    const dictionary = state.dictionaries && state.dictionaries.find(item => item.name === name);
    return dictionary ? dictionary.canImportFromExcel : false
});

export const importProgressSelector = createSelector(stateSelector, state => state.importProgress);

//*  SAGA  *//

export function* getListSaga({ payload }) {
    try {
        const { filter = {}, name, isConcat } = payload;

        yield delay(1000);

        const result = yield postman.post(`/${name}/search`, filter);

        yield put({ type: GET_DICTIONARY_LIST_SUCCESS, payload: { ...result, isConcat } });
    } catch (error) {
        yield put({ type: GET_DICTIONARY_LIST_ERROR, payload: error });
    }
}

function* getCardSaga({ payload }) {
    try {
        const { name, id } = payload;
        const result = yield postman.get(`${name}/getById/${id}`);
        yield put({ type: GET_DICTIONARY_CARD_SUCCESS, payload: result });
    } catch (error) {
        yield put({ type: GET_DICTIONARY_CARD_ERROR });
    }
}

function* saveDictionaryCardSaga ({ payload }) {
    try {
        const {params, name, callbackSuccess} = payload;
        const result = yield postman.post(`/${name}/saveOrCreate`, params);

        yield put({
            type: SAVE_DICTIONARY_CARD_SUCCESS
        });

        callbackSuccess && callbackSuccess();
    } catch (e) {
        yield put({
            type: SAVE_DICTIONARY_CARD_ERROR,
            payload: e
        })
    }
}

function* importFromExcelSaga({ payload }) {
    try {
        const {form, name} = payload;
        const result = yield postman.post(`${name}/importFromExcel`, form, {
            headers: { 'Content-Type': 'multipart/form-data' },
        });

        yield put({
            type: DICTIONARY_IMPORT_FROM_EXCEL_SUCCESS
        })
    } catch (e) {
        yield put({
            type: DICTIONARY_IMPORT_FROM_EXCEL_ERROR
        })
    }
}

export function* saga() {
    yield all([
        takeEvery(GET_DICTIONARY_LIST_REQUEST, getListSaga),
        takeEvery(GET_DICTIONARY_CARD_REQUEST, getCardSaga),
        takeEvery(SAVE_DICTIONARY_CARD_REQUEST, saveDictionaryCardSaga),
        takeEvery(DICTIONARY_IMPORT_FROM_EXCEL_REQUEST, importFromExcelSaga),
    ]);
}
