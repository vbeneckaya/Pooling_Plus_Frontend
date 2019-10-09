import { all } from 'redux-saga/effects';
import { saga as profileSaga } from '../ducks/profile';
import { saga as loginSaga } from '../ducks/login';
import { saga as rolesSaga } from '../ducks/roles';
import { saga as gridListSaga } from '../ducks/gridList';
import { saga as dictionaryViewSaga } from '../ducks/dictionaryView';
import { saga as gridActionsSaga } from '../ducks/gridActions';
import { saga as gridCardSaga } from '../ducks/gridCard';
import { saga as lookupSaga } from '../ducks/lookup';
import { saga as documentsSaga } from '../ducks/documents';
import { saga as representationsSaga } from '../ducks/representations';
import { saga as historySaga } from '../ducks/history';
import { saga as usersSaga } from '../ducks/users';

export default function* rootSaga() {
    yield all([
        historySaga(),
        representationsSaga(),
        documentsSaga(),
        lookupSaga(),
        gridCardSaga(),
        gridActionsSaga(),
        dictionaryViewSaga(),
        gridListSaga(),
        profileSaga(),
        loginSaga(),
        rolesSaga(),
        usersSaga(),
    ]);
}
