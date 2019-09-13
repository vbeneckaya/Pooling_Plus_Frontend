import { all, takeEvery, delay, put } from 'redux-saga/effects';
import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import profile from '../mocks/profile';

//*  TYPES  *//
const GET_USER_PROFILE_REQUEST = 'GET_USER_PROFILE_REQUEST';
const GET_USER_PROFILE_SUCCESS = 'GET_USER_PROFILE_SUCCESS';
const GET_USER_PROFILE_ERROR = 'GET_USER_PROFILE_ERROR';

//*  INITIAL STATE  *//

const initial = {
    progress: false,
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        case GET_USER_PROFILE_REQUEST:
            return {
                ...state,
                progress: true,
            };
        case GET_USER_PROFILE_SUCCESS:
            return {
                ...state,
                progress: false,
                ...payload,
            };
        case GET_USER_PROFILE_ERROR:
            return {
                ...state,
                progress: false,
            };
        default:
            return state;
    }
};

//*  ACTION CREATORS  *//

export const getUserProfile = payload => {
    return {
        type: GET_USER_PROFILE_REQUEST,
        payload,
    };
};

//*  SELECTORS *//

const stateSelector = state => state.profile;
export const gridsMenuSelector = createSelector(
    stateSelector,
    state => state.grids && state.grids.map(grid => grid.name),
);
export const dictionariesMenuSelector = createSelector(
    stateSelector,
    state => state.dictionaries && state.dictionaries.map(dictionary => dictionary.name),
);
export const userNameSelector = createSelector(
    stateSelector,
    state => state.userName,
);
export const roleSelector = createSelector(
    stateSelector,
    state => state.userRole,
);
export const homePageSelector = createSelector(
    stateSelector,
    state => (state.grids && state.grids.length ? state.grids[0].name : ''),
);

//*  SAGA  *//

function* getUserProfileSaga({ payload }) {
    try {
        const userInfo = yield postman.get('/identity/userInfo');
        const config = yield postman.get('/appConfiguration');
        yield put({
            type: GET_USER_PROFILE_SUCCESS,
            payload: {...userInfo, ...config},
        });
    } catch (e) {
        yield put({
            type: GET_USER_PROFILE_ERROR,
            payload: e,
        });
    }
}

export function* saga() {
    yield all([takeEvery(GET_USER_PROFILE_REQUEST, getUserProfileSaga)]);
}
