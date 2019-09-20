import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import { all, takeEvery, put, call } from 'redux-saga/effects';
import { newExpression } from '@babel/types';

//*  TYPES  *//

const OPEN_GRID_CARD_REQUEST = 'OPEN_GRID_CARD_REQUEST';
const OPEN_GRID_CARD_SUCCESS = 'OPEN_GRID_CARD_SUCCESS';
const OPEN_GRID_CARD_ERROR = 'OPEN_GRID_CARD_ERROR';

const CREATE_DRAFT_REQUEST = 'CREATE_DRAFT_REQUEST';
const CREATE_DRAFT_SUCCESS = 'CREATE_DRAFT_SUCCESS';
const CREATE_DRAFT_ERROR = 'CREATE_DRAFT_ERROR';

const GET_CARD_CONFIG_REQUEST = 'GET_CARD_CONFIG_REQUEST';
const GET_CARD_CONFIG_SUCCESS = 'GET_CARD_CONFIG_SUCCESS';
const GET_CARD_CONFIG_ERROR = 'GET_CARD_CONFIG_ERROR';

const GET_GRID_CARD_REQUEST = 'GET_GRID_CARD_REQUEST';
const GET_GRID_CARD_SUCCESS = 'GET_GRID_CARD_SUCCESS';
const GET_GRID_CARD_ERROR = 'GET_GRID_CARD_ERROR';

//*  INITIAL STATE  *//

const initial = {
    config: {},
    data: {},
    progress: false,
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_GRID_CARD_REQUEST:
        case CREATE_DRAFT_REQUEST:
        case GET_CARD_CONFIG_REQUEST:
            return {
                ...state,
                progress: true,
            };
        case GET_GRID_CARD_SUCCESS:
            return {
                ...state,
                progress: false,
                data: payload,
            };
        case CREATE_DRAFT_SUCCESS:
            return {
                ...state,
                progress: false,
                data: { id: payload },
            };
        case GET_CARD_CONFIG_SUCCESS:
            return {
              ...state,
              progress: false,
              config: payload
            };
        case GET_GRID_CARD_ERROR:
        case CREATE_DRAFT_ERROR:
        case GET_CARD_CONFIG_ERROR:
            return {
                ...state,
                data: {},
                progress: false,
            };
        case OPEN_GRID_CARD_REQUEST:
            return {
                ...state,
                progress: true,
            };
        case OPEN_GRID_CARD_SUCCESS:
        case OPEN_GRID_CARD_ERROR:
            return {
                ...state,
                progress: false,
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const openGridCardRequest = payload => {
    return {
        type: OPEN_GRID_CARD_REQUEST,
        payload,
    };
};

export const createDraftRequest = payload => {
    return {
        type: CREATE_DRAFT_REQUEST,
        payload,
    };
};

export const getCardRequest = payload => {
    return {
        type: GET_GRID_CARD_REQUEST,
        payload,
    };
};

export const getCardConfigRequest = payload => {
    return {
        type: GET_CARD_CONFIG_REQUEST,
        payload,
    };
};

//*  SELECTORS *//

const stateSelector = state => state.gridCard;

const gridName = (state, name) => name;

const configSelector = createSelector(stateSelector, state => state.config);

export const titleCardSelector = createSelector(configSelector, state => state.title);

export const tabsSelector = createSelector(configSelector, state => {
    const tabs = state.tabs;

    return tabs ? tabs.map(item => item.name) : [];
});

export const cardSelector = createSelector(
    stateSelector,
    state => state.card,
);

//*  SAGA  *//

function* openGridCardSaga({ payload }) {
    try {
        const { name } = payload;

        yield call(createDraftSaga, { payload: { name } });

        yield call(getCardConfigSaga, { payload: { name, id: '1' } });

        yield call(getCardSaga, { payload: { name, id: '1' } });

    } catch (error) {
        yield put({
            type: OPEN_GRID_CARD_ERROR,
            payload: error,
        });
    }
}

function* createDraftSaga({ payload }) {
    try {
        const { name } = payload;
        const result = yield postman.post(`/${name}/saveOrCreate`, {});

        yield put({
            type: CREATE_DRAFT_SUCCESS,
            payload: result,
        });
    } catch (error) {
        yield put({
            type: CREATE_DRAFT_ERROR,
            payload: error,
        });
    }
}

function* getCardConfigSaga({ payload }) {
    try {
        console.log('payload', payload);
        const { name, id } = payload;
        const result = yield postman.get(`/getFormFor/${name}/${id}`);

        yield put({
            type: GET_CARD_CONFIG_SUCCESS,
            payload: result,
        });
    } catch (error) {
        yield put({
            type: GET_CARD_CONFIG_ERROR,
            payload: error,
        });
    }
}

function* getCardSaga({ payload }) {
    try {
        const { name, id } = payload;
        const result = yield postman.get(`${name}/${id}`);
        yield put({ type: GET_GRID_CARD_SUCCESS, payload: result.card });
    } catch (error) {
        yield put({ type: GET_GRID_CARD_ERROR });
    }
}

export function* saga() {
    yield all([
        takeEvery(OPEN_GRID_CARD_REQUEST, openGridCardSaga),
        takeEvery(CREATE_DRAFT_REQUEST, createDraftSaga),
        takeEvery(GET_CARD_CONFIG_REQUEST, getCardConfigSaga),
        takeEvery(GET_GRID_CARD_REQUEST, getCardSaga),
    ]);
}
