import { createSelector } from 'reselect';
import { postman } from '../utils/postman';
import { all, takeEvery, put, cancelled, delay, fork, cancel, select } from 'redux-saga/effects';
//*  TYPES  *//

const GET_FIELDS_SETTINGS_REQUEST = 'GET_FIELDS_SETTINGS_REQUEST';
const GET_FIELDS_SETTINGS_SUCCESS = 'GET_FIELDS_SETTINGS_SUCCESS';
const GET_FIELDS_SETTINGS_ERROR = 'GET_FIELDS_SETTINGS_ERROR';

//*  INITIAL STATE  *//

const initial = {
  settings: []
};

//*  REDUCER  *//

export default (state = initial, { type, payload }) => {
    switch (type) {
        default:
            return state;
    }
}

//*  ACTION CREATORS  *//

export const getFieldsSettingRequest = payload => {
    return {
        type: GET_FIELDS_SETTINGS_REQUEST,
        payload
    }
};

//*  SELECTORS *//

function* getFieldsSettingSaga({ payload }) {
    try {

    } catch (e) {
        yield put({
            type: GET_FIELDS_SETTINGS_ERROR,
            payload: e
        })
    }
}

//*  SAGA  *//

export function* saga() {}
