import React, { useEffect } from 'react';
import { Switch, Redirect, Route } from 'react-router-dom';
import { withRouter } from 'react-router';
import { useSelector } from 'react-redux';
import { GRID_LINK, LOGIN_LINK, ROLES_LINK, USERS_LINK } from './links';
import CustomGrid from '../containers/customGrid/list';
import CustomDictionary from '../containers/customDictionary/list';
import PrivateRoute from './privateRoute';
import Login from '../containers/login';
import RolesList from '../containers/roles/roles_list';
import UsersList from '../containers/users/users_list';
import { homePageSelector } from '../ducks/profile';

const MainRoute = withRouter(props => {
    const homePage = useSelector(state => homePageSelector(state));

    useEffect(() => {
        const { history, location } = props;
        const { pathname } = location;
        if (pathname === '/grid/' && homePage) {
            history.push(`/grid/${homePage}`);
        }
    }, [homePage]);

    return (
        <Switch>
            <PrivateRoute
                exact
                path="/"
                component={() => <Redirect to={GRID_LINK.replace(':name', homePage)} />}
            />
            <PrivateRoute exact path="/grid/:name" component={() => <CustomGrid />} />
            <PrivateRoute exact path="/dictionary/:name" component={() => <CustomDictionary />} />
            <PrivateRoute exact path={ROLES_LINK} component={() => <RolesList />} />
            <PrivateRoute exact path={USERS_LINK} component={() => <UsersList />} />
            <Route exact path={LOGIN_LINK} component={Login} />
        </Switch>
    );
});

export default MainRoute;
