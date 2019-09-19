import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import { all, takeEvery, put, cancelled, delay, fork, cancel } from 'redux-saga/effects';

//*  TYPES  *//

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
    progress: false
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_GRID_CARD_REQUEST:
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
        case GET_GRID_CARD_ERROR:
            return {
                ...state,
                data: {},
                progress: false,
            };
        default:
            return state;
    }
}

//*  ACTION CREATORS  *//

export const createDraftRequest = payload => {
    return {
        type: CREATE_DRAFT_REQUEST,
        payload
    }
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
        payload
    }
};


//*  SELECTORS *//

const stateSelector = state => state.gridCard;

const gridName = (state, name) => name;

export const cardSelector = createSelector(
    stateSelector,
    state => state.card,
);

//*  SAGA  *//

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
        takeEvery(GET_GRID_CARD_REQUEST, getCardSaga),
    ])
}
