import { all } from 'redux-saga/effects';
import { saga as loginSaga } from '../ducks/login';

export default function* rootSaga() {
    yield all([loginSaga()]);
}
