import React, { Suspense } from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import './utils/i18n';

import store from './store/configureStore';

import App from './containers/App';
import {Dimmer, Loader} from "semantic-ui-react";

String.prototype.replaceAll = function(search, replacement) {
    const target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

ReactDOM.render(
    <Provider store={store}>
        <Suspense fallback={<Dimmer active inverted className="table-loader">
            <Loader size="huge">Loading</Loader>
        </Dimmer>}>
            <App />
        </Suspense>
    </Provider>,
    document.getElementById('root'),
);
