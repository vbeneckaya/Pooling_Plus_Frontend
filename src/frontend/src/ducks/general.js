import {select, take, spawn, all, put, takeEvery} from 'redux-saga/effects';
import {createSelector} from 'reselect';
import {clearDictionaryInfo} from './dictionaryView';
import {autoUpdateStop} from './gridList';
import React from "react";
import InstructionContent from "../components/InstructionModal/instructions";

const SHOW_INSTRUCTION_SUCCESS = 'SHOW_INSTRUCTION_SUCCESS';

const HIDE_INSTRUCTION = 'HIDE_INSTRUCTION';

//*  INITIAL STATE  *//

const initial = {
    card: null,
};

export default (state = initial, {type, payload}) => {
    switch (type) {
        case SHOW_INSTRUCTION_SUCCESS:
            return {
                ...state,
                card: payload,
            };
        case HIDE_INSTRUCTION:
            return {
                ...state,
                card: null,
            };

        default:
            return state;
    }
};

const routeSelector = state => state.router;
const instructionSelector = state => state.instructions;

const currentLocationSelector = createSelector(routeSelector, state => state.location.pathname);
export const showInstructionSelector = createSelector(instructionSelector, state => state.card);

function* changeLocation() {
    while (true) {
        const currentLocation = yield select(currentLocationSelector);
        const {payload} = yield take('@@router/LOCATION_CHANGE');
        const {location} = payload;
        const {pathname} = location;

        const content = InstructionContent(pathname)
        if (!content) {
            yield put({
                type: SHOW_INSTRUCTION_SUCCESS,
                payload: null,
            });
        }
        else {
            const alreadyInLocalStorage = localStorage.getItem(pathname);

            if (content && !alreadyInLocalStorage) {
                localStorage.setItem(pathname, '1');
                yield put({
                    type: SHOW_INSTRUCTION_SUCCESS,
                    payload: content,
                });
            }
        }

        if (pathname.includes('dictionary') && pathname !== currentLocation) {
            yield put(clearDictionaryInfo());
        }

        if (pathname.includes('grid') && pathname !== currentLocation) {
            yield put(
                autoUpdateStop({
                    isClear: true,
                }),
            );
        }
    }
}

export const hideInstruction = payload => {
    return {
        type: HIDE_INSTRUCTION,
        payload,
    };
};

function* hideInstructionSaga() {
    yield put({
        type: HIDE_INSTRUCTION
    });
}

export function* saga() {
    yield spawn(changeLocation);
    yield all(takeEvery(HIDE_INSTRUCTION, hideInstructionSaga));
}
