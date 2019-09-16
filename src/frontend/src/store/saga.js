import { all } from 'redux-saga/effects';
import { saga as profileSaga } from '../ducks/profile';
import { saga as loginSaga } from '../ducks/login';
import { saga as rolesSaga } from '../ducks/roles';
import { saga as gridViewSaga } from '../ducks/gridView';
import { saga as dictionaryViewSaga } from '../ducks/dictionaryView';
import { saga as gridActionsSaga } from '../ducks/gridActions';
import { saga as usersSaga } from '../ducks/users';

export default function* rootSaga() {
    yield all([
        gridActionsSaga(),
        dictionaryViewSaga(),
        gridViewSaga(),
        profileSaga(),
        loginSaga(),
        rolesSaga(),
        usersSaga(),
    ]);
}
