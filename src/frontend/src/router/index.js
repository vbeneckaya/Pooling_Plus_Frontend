import React from 'react';
import { Switch, Redirect } from 'react-router-dom';
import { LOGIN_LINK, TEST_LINK } from './links';
import PrivateRoute from './privateRoute';
import Login from '../containers/login';

export function MainRoute() {
    return (
        <Switch>
            <PrivateRoute exact path={LOGIN_LINK} component={Login} />
        </Switch>
    );
}
