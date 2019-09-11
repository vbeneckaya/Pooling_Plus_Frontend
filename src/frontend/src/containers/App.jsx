import React, { Component } from 'react';
import { history } from '../../src/store/configureStore';
import { ConnectedRouter } from 'connected-react-router';
import { MainRoute } from '../router';
import Header from '../components/Header';

import 'semantic-ui-css/semantic.min.css';
import 'react-datepicker/dist/react-datepicker.min.css';
import 'react-toastify/dist/ReactToastify.min.css';
import '../style/main.scss';
import ToastPortalContainer from '../components/ToastContainer';

class App extends Component {
    render() {
        return (
            <>
                <ConnectedRouter history={history}>
                    <Header />
                    <MainRoute />
                </ConnectedRouter>
                <ToastPortalContainer />
            </>
        );
    }
}

export default App;
