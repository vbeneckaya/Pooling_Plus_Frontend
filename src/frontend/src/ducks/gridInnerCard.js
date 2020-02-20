import {createSelector} from 'reselect';
import {postman} from '../utils/postman';
import {all, call, fork, put, select, takeEvery} from 'redux-saga/effects';
import {toast} from 'react-toastify';
import {roleIdSelector} from './profile';
import {
    fieldsSettingSelector,
    fieldsExtSettingSelector,
    getFieldsSettingSaga,
    getShippingOrderFieldsSettingSaga
} from './fieldsSetting';
import {SETTINGS_TYPE_HIDE} from '../constants/formTypes';
import {errorMapping} from '../utils/errorMapping';
import {ORDERS_GRID, SHIPPINGS_GRID} from "../constants/grids";

//*  TYPES  *//

// const OPEN_GRID_INNER_CARD_REQUEST = 'OPEN_GRID_INNER_CARD_REQUEST';
// const OPEN_GRID_INNER_CARD_SUCCESS = 'OPEN_GRID_INNER_CARD_SUCCESS';
// const OPEN_GRID_INNER_CARD_ERROR = 'OPEN_GRID_INNER_CARD_ERROR';
//
// const CREATE_DRAFT_REQUEST = 'CREATE_DRAFT_REQUEST';
// const CREATE_DRAFT_SUCCESS = 'CREATE_DRAFT_SUCCESS';
// const CREATE_DRAFT_ERROR = 'CREATE_DRAFT_ERROR';
//
const GET_INNER_CARD_CONFIG_REQUEST = 'GET_INNER_CARD_CONFIG_REQUEST';
const GET_INNER_CARD_CONFIG_SUCCESS = 'GET_INNER_CARD_CONFIG_SUCCESS';
const GET_INNER_CARD_CONFIG_ERROR = 'GET_INNER_CARD_CONFIG_ERROR';

const GET_GRID_INNER_CARD_REQUEST = 'GET_GRID_INNER_CARD_REQUEST';
const GET_GRID_INNER_CARD_SUCCESS = 'GET_GRID_INNER_CARD_SUCCESS';
const GET_GRID_INNER_CARD_ERROR = 'GET_GRID_INNER_CARD_ERROR';

const EDIT_GRID_INNER_CARD_REQUEST = 'EDIT_GRID_INNER_CARD_REQUEST';
const EDIT_GRID_INNER_CARD_SUCCESS = 'EDIT_GRID_INNER_CARD_SUCCESS';
const EDIT_GRID_INNER_CARD_ERROR = 'EDIT_GRID_INNER_CARD_ERROR';

const IS_UNIQUE_NUMBER_INNER_REQUEST = 'IS_UNIQUE_NUMBER_INNER_REQUEST';
const IS_UNIQUE_NUMBER_INNER_SUCCESS = 'IS_UNIQUE_NUMBER_INNER_SUCCESS';
const IS_UNIQUE_NUMBER_INNER_ERROR = 'IS_UNIQUE_NUMBER_INNER_ERROR';

const CLEAR_GRID_INNER_CARD = 'CLEAR_GRID_INNER_CARD';

const ADD_ERROR = 'ADD_ERROR';
const CLEAR_ERROR = 'CLEAR_ERROR';

//*  INITIAL STATE  *//

const initial = {
    config: {},
    data: {},
    error: [],
    progress: false,
    editProgress: false,
};

//*  REDUCER  *//

export default (state = initial, {type, payload}) => {
    switch (type) {
        case GET_GRID_INNER_CARD_REQUEST:
      //  case CREATE_DRAFT_REQUEST:
        case GET_INNER_CARD_CONFIG_REQUEST:
            return {
                ...state,
                progress: true,
            };
        case GET_GRID_INNER_CARD_SUCCESS:
            return {
                ...state,
                progress: false,
                data: payload,
            };
        // case CREATE_DRAFT_SUCCESS:
        //     return {
        //         ...state,
        //         progress: false,
        //         data: {id: payload.id},
        //     };
        case GET_INNER_CARD_CONFIG_SUCCESS:
            return {
                ...state,
                progress: false,
                config: payload,
            };
        case GET_GRID_INNER_CARD_ERROR:
    //    case CREATE_DRAFT_ERROR:
        case GET_INNER_CARD_CONFIG_ERROR:
            return {
                ...state,
                data: {},
                progress: false,
            };
        // case OPEN_INNER_GRID_CARD_REQUEST:
        //     return {
        //         ...state,
        //         progress: true,
        //         data: {
        //             id: payload.id,
        //         },
        //     };
        // case OPEN_GRID_CARD_SUCCESS:
        // case OPEN_GRID_CARD_ERROR:
        //     return {
        //         ...state,
        //         progress: false,
        //     };
        case EDIT_GRID_INNER_CARD_REQUEST:
            return {
                ...state,
                editProgress: true,
            };
        case EDIT_GRID_INNER_CARD_SUCCESS:

            return {
                ...state,
                error: [],
                editProgress: false,
                data: {...state.data, id: payload.id},
            };
        case EDIT_GRID_INNER_CARD_ERROR:
            return {
                ...state,
                error: payload,
                editProgress: false,
            };
        case IS_UNIQUE_NUMBER_INNER_SUCCESS:
            return {
                ...state,
                error: [
                    ...state.error,
                    {
                        name: payload.fieldName,
                        message: payload.errorText,
                    },
                ],
            };
        case CLEAR_GRID_INNER_CARD:
            return {
                ...state,
                error: [],
                data: {},
            };
        case CLEAR_ERROR:
            return {
                ...state,
                error: state.error && state.error.filter(item => item.name !== payload),
            };
        case ADD_ERROR:
            return {
                ...state,
                error: [...state.error, payload],
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

// export const openGridInnerCardRequest = payload => {
//     return {
//         type: OPEN_GRID_INNER_CARD_REQUEST,
//         payload,
//     };
// };

// export const createDraftRequest = payload => {
//     return {
//         type: CREATE_DRAFT_REQUEST,
//         payload,
//     };
// };

export const getInnerCardRequest = payload => {
    return {
        type: GET_GRID_INNER_CARD_REQUEST,
        payload,
    };
};

export const getInnerCardConfigRequest = payload => {
    return {
        type: GET_INNER_CARD_CONFIG_REQUEST,
        payload,
    };
};

export const editInnerCardRequest = payload => {
    return {
        type: EDIT_GRID_INNER_CARD_REQUEST,
        payload,
    };
};

export const isUniqueNumberInnerRequest = payload => {
    return {
        type: IS_UNIQUE_NUMBER_INNER_REQUEST,
        payload,
    };
};

export const clearGridInnerCard = () => {
    return {
        type: CLEAR_GRID_INNER_CARD,
    };
};

export const clearInnerCardError = payload => {
    return {
        type: CLEAR_ERROR,
        payload,
    };
};

export const addInnerCardError = payload => {
    return {
        type: ADD_ERROR,
        payload,
    };
};

//*  SELECTORS *//

const stateSelector = state => state.gridInnerCard;

const idSelector = createSelector(stateSelector, state => state.data.id);

export const progressInnerSelector = createSelector(stateSelector, state => state.progress);
export const editProgressInnerSelector = createSelector(stateSelector, state => state.editProgress);

export const innerCardSelector = createSelector(stateSelector, state => state.data);

export const settingsFormInnerSelector = createSelector(
    [fieldsSettingSelector, (state, status) => status],
    (state, status) => {
        let settings = {};
        const {base = []} = state;
        base.forEach(item => {
            settings = {
                ...settings,
                [item.fieldName]: item.isHidden ? SETTINGS_TYPE_HIDE : item.accessTypes[status],
            };
        });

        return settings;
    },
);

export const settingsFormExtInnerSelector = createSelector(
    [fieldsExtSettingSelector, (state, status) => status],
    (state, status) => {
        let settings = {};
        const {base = []} = state;
        base.forEach(item => {
            settings = {
                ...settings,
                [item.fieldName]: item.isHidden ? SETTINGS_TYPE_HIDE : item.accessTypes[status],
            };
        });

        return settings;
    },
);

export const settingsExtInnerSelector = createSelector(
    [fieldsSettingSelector, (state, status) => status],
    (state, status) => {
        let settings = {};
        const {ext = []} = state;
        ext.forEach(item => {
            settings = {
                ...settings,
                [item.fieldName]: item.isHidden ? SETTINGS_TYPE_HIDE : item.accessTypes[status],
            };
        });
        return settings;
    },
);

export const errorInnerSelector = createSelector(stateSelector, state => errorMapping(state.error));

//*  SAGA  *//

// function* openGridInnerCardSaga({payload}) {
//     try {
//         const {name, id: idRow, callbackSuccess} = payload;
//
//         if (!idRow) {
//             yield call(createDraftSaga, {payload: {name}});
//         }
//
//         const id = yield select(idSelector);
//
//         // yield call(getCardConfigSaga, { payload: { name, id } });
//
//         yield call(getCardSaga, {payload: {name, id}});
//
//         const card = yield select(innerCardSelector);
//
//         callbackSuccess(card);
//     } catch (error) {
//         yield put({
//             type: OPEN_GRID_INNER_CARD_ERROR,
//             payload: error,
//         });
//     }
// }

// function* createDraftSaga({payload}) {
//     try {
//         const {name} = payload;
//         const result = yield postman.post(`/${name}/saveOrCreate`, {});
//
//         yield put({
//             type: CREATE_DRAFT_SUCCESS,
//             payload: result,
//         });
//     } catch (error) {
//         yield put({
//             type: CREATE_DRAFT_ERROR,
//             payload: error,
//         });
//     }
// }

function* editInnerCardSaga({payload}) {
    try {
        const {name, params, callbackSuccess} = payload;
        const result = yield postman.post(`/${name}/saveOrCreate`, params);

        if (result.isError) {
            toast.error(result.error);
            yield put({
                type: EDIT_GRID_INNER_CARD_ERROR,
                payload: result.errors,
            });
        } else {
            yield put({
                type: EDIT_GRID_INNER_CARD_SUCCESS,
                payload: result,
            });

            callbackSuccess && callbackSuccess(result);
        }
    } catch (error) {
        yield put({
            type: EDIT_GRID_INNER_CARD_ERROR,
        });
    }
}

function* getInnerCardConfigSaga({payload}) {
    try {
        const {name, id} = payload;
        const result = yield postman.get(`/getFormFor/${name}/${id}`);

        yield put({
            type: GET_INNER_CARD_CONFIG_SUCCESS,
            payload: result,
        });
    } catch (error) {
        yield put({
            type: GET_INNER_CARD_CONFIG_ERROR,
            payload: error,
        });
    }
}

function* getInnerCardSaga({payload}) {
    try {
        const {name, id, callbackSuccess} = payload;
        const roleId = yield select(state => roleIdSelector(state));
        yield fork(getFieldsSettingSaga, {
            payload: {
                forEntity: name,
                roleId,
            },
        });

        if (name === SHIPPINGS_GRID)
            yield fork(getShippingOrderFieldsSettingSaga, {
                payload: {
                    forEntity: ORDERS_GRID,
                    roleId,
                },
            });
        const result = yield postman.get(`${name}/getById/${id}`);
        yield put({type: GET_GRID_INNER_CARD_SUCCESS, payload: result});
        callbackSuccess && callbackSuccess(result);
    } catch (error) {
        yield put({type: GET_GRID_INNER_CARD_ERROR});
    }
}

function* isUniqueNumberInnerSaga({payload}) {
    try {
        const {number, fieldName, errorText, callbackSuccess} = payload;
        const result = yield postman.post('/orders/findNumber', {number, isPartial: false});

        if (result.length && result[0].name) {
            yield put({
                type: IS_UNIQUE_NUMBER_INNER_SUCCESS,
                payload: {
                    fieldName,
                    errorText,
                },
            });
        } else {
            callbackSuccess && callbackSuccess();
        }
    } catch (e) {
        yield put({
            type: IS_UNIQUE_NUMBER_INNER_ERROR,
        });
    }
}

export function* saga() {
    yield all([
        //takeEvery(OPEN_GRID_INNER_CARD_REQUEST, openGridInnerCardSaga),
        // takeEvery(CREATE_DRAFT_REQUEST, createDraftSaga),
        takeEvery(GET_INNER_CARD_CONFIG_REQUEST, getInnerCardConfigSaga),
        takeEvery(GET_GRID_INNER_CARD_REQUEST, getInnerCardSaga),
        takeEvery(EDIT_GRID_INNER_CARD_REQUEST, editInnerCardSaga),
        takeEvery(IS_UNIQUE_NUMBER_INNER_REQUEST, isUniqueNumberInnerSaga),
    ]);
}
