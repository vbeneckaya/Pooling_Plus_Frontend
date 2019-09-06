import React, { Component } from 'react';
import { history } from '../../src/store/configureStore';
import { ConnectedRouter } from 'connected-react-router';
import { MainRoute } from '../router';
import Header from '../components/Header';

import 'semantic-ui-css/semantic.min.css';
import '../style/main.scss';

class App extends Component {
    render() {
        return (
            <ConnectedRouter history={history}>
                <Header />
                <MainRoute />
            </ConnectedRouter>
        );
    }
}

export default App;
