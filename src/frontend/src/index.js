import React, { Suspense } from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import './utils/i18n';

import store from './store/configureStore';

import App from './containers/App';

ReactDOM.render(
    <Provider store={store}>
        <Suspense fallback="loading">
            <App />
        </Suspense>
    </Provider>,
    document.getElementById('root'),
);
