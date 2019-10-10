import { createSelector } from 'reselect';
import { all, takeEvery, select } from 'redux-saga/effects';
import { columnsGridSelector } from './gridList';

//*  TYPES  *//
const SAVE_REPRESENTATION_REQUEST = 'SAVE_REPRESENTATION_REQUEST';
const SAVE_REPRESENTATION_SUCCESS = 'SAVE_REPRESENTATION_SUCCESS';
const SAVE_REPRESENTATION_ERROR = 'SAVE_REPRESENTATION_ERROR';

const EDIT_REPRESENTATION_REQUEST = 'EDIT_REPRESENTATION_REQUEST';

const SET_REPRESENTATION = 'SET_REPRESENTATION';

const REPRESENTATIONS_KEY = 'representations';
const REPRESENTATION_KEY = 'representation';

//*  INITIAL STATE  *//

const initial = {
    list: JSON.parse(localStorage.getItem(REPRESENTATIONS_KEY)) || {},
    representation: JSON.parse(localStorage.getItem(REPRESENTATION_KEY)) || {},
};

//*  REDUCER  *//

export default (state = initial, { type, payload = {} }) => {
    const { gridName, name, value, oldName } = payload;

    switch (type) {
        case SAVE_REPRESENTATION_REQUEST:
            return {
                ...state,
                list: {
                    ...state.list,
                    [gridName]: {
                        ...state.list[gridName],
                        [name]: value,
                    },
                },
            };
        case EDIT_REPRESENTATION_REQUEST:
            delete state.list[gridName][oldName];
            return {
                ...state,
                list: {
                    ...state.list,
                    [gridName]: {
                        ...state.list[gridName],
                        [name]: value,
                    },
                },
            };
        case SET_REPRESENTATION:
            return {
                ...state,
                representation: {
                    ...state.representation,
                    [gridName]: value,
                },
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const saveRepresentationRequest = payload => {
    return {
        type: SAVE_REPRESENTATION_REQUEST,
        payload,
    };
};

export const editRepresentationRequest = payload => {
    return {
        type: EDIT_REPRESENTATION_REQUEST,
        payload
    }
};

export const setRepresentationRequest = payload => {
    return {
        type: SET_REPRESENTATION,
        payload,
    };
};

//*  SELECTORS *//

export const stateSelector = state => state.representations;

export const representationsSelector = createSelector(
    [stateSelector, (state, name) => name],
    (state, gridName) => {
        const { list = {} } = state;

        return list[gridName];
    },
);

export const representationNameSelector = createSelector(
    [stateSelector, (state, name) => name],
    (state, gridName) => {
        return state.representation[gridName];
    },
);

export const representationSelector = createSelector(
    [stateSelector, (state, name) => name],
    (state, gridName) => {
        const representationName = state.representation[gridName];
        return representationName ? state.list[gridName][representationName] : [];
    },
);

export const representationFromGridSelector = createSelector(
    [stateSelector, (state, name) => name, (state, name) => columnsGridSelector(state, name)],
    (state, gridName, list) => {
        const representationName = state.representation[gridName];
        if (representationName) {
            return state.list[gridName][representationName];
        }
        return list;
    },
);

//*  SAGA  *//

function* saveRepresentationSaga({ payload }) {
    try {
        const { callbackSuccess } = payload;
        const state = yield select(state => state.representations.list);
        localStorage.setItem(REPRESENTATIONS_KEY, JSON.stringify(state));
        callbackSuccess && callbackSuccess();
    } catch (e) {
        console.log('___error', e);
    }
}


function* editRepresentationSaga({ payload }) {
    try {
        const { callbackSuccess } = payload;
        const state = yield select(state => state.representations.list);
        localStorage.setItem(REPRESENTATIONS_KEY, JSON.stringify(state));
        callbackSuccess && callbackSuccess();
    } catch (e) {
        console.log('___error', e);
    }
}

function* setRepresentationSaga({ paylod }) {
    try {
        const state = yield select(state => state.representations.representation);
        localStorage.setItem(REPRESENTATION_KEY, JSON.stringify(state));
    } catch (e) {
        console.log('___error', e);
    }
}

export function* saga() {
    yield all([
        takeEvery(SAVE_REPRESENTATION_REQUEST, saveRepresentationSaga),
        takeEvery(EDIT_REPRESENTATION_REQUEST, editRepresentationSaga),
        takeEvery(SET_REPRESENTATION, setRepresentationSaga),
    ]);
}
