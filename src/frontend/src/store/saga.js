import { all } from 'redux-saga/effects';
import { saga as profileSaga } from '../ducks/profile';
import { saga as loginSaga } from '../ducks/login';
import { saga as rolesSaga } from '../ducks/roles';
import { saga as usersSaga } from '../ducks/users';

export default function* rootSaga() {
    yield all([profileSaga(), loginSaga(), rolesSaga(), usersSaga()]);
}
