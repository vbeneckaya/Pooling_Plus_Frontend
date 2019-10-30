import React, { useEffect } from 'react';
import { history } from '../../src/store/configureStore';
import { ConnectedRouter } from 'connected-react-router';
import MainRoute from '../router';
import Header from '../components/Header';

import { useSelector, useDispatch } from 'react-redux';

import 'semantic-ui-css/semantic.min.css';
import 'react-datepicker/dist/react-datepicker.min.css';
import 'react-toastify/dist/ReactToastify.min.css';
import 'react-resizable/css/styles.css';
import '../style/main.scss';
import ToastPortalContainer from '../components/ToastContainer';
import { getUserProfile, userNameSelector } from '../ducks/profile';
import { isAuthSelector } from '../ducks/login';
import { Dimmer, Loader } from 'semantic-ui-react';

const App = () => {
    const dispatch = useDispatch();
    const userName = useSelector(state => userNameSelector(state));
    const isAuth = useSelector(state => isAuthSelector(state));

    const getProfile = () => {
        if (!userName && isAuth) {
            dispatch(getUserProfile());
        }
    };

    useEffect(getProfile, []);

    return (
        <>
            <ConnectedRouter history={history}>
                {userName ? (
                    <>
                        <Header />
                        <MainRoute />
                    </>
                ) : (
                    <Dimmer active inverted className="table-loader">
                        <Loader size="huge">Loading</Loader>
                    </Dimmer>
                )}
            </ConnectedRouter>
            <ToastPortalContainer />
        </>
    );
};

export default App;
