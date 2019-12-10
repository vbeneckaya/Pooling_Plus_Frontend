import React, { useEffect } from 'react';
import { Switch, Redirect, Route } from 'react-router-dom';
import { withRouter } from 'react-router';
import { useSelector } from 'react-redux';
import {
    DICTIONARY_CARD_LINK,
    DICTIONARY_LIST_LINK, DICTIONARY_NEW_LINK,
    FIELDS_SETTING_LINK,
    GRID_LINK,
    LOGIN_LINK, NEW_USER_LINK,
    ROLES_LINK, USER_LINK,
    USERS_LINK
} from './links';
import CustomGrid from '../containers/customGrid/list';
import CustomDictionaryList from '../containers/customDictionary/list';
import CustomDictionaryCard from '../containers/customDictionary/card_new';
import PrivateRoute from './privateRoute';
import Login from '../containers/login';
import RolesList from '../containers/roles/roles_list';
import UsersList from '../containers/users/users_list';
import UserCard from '../containers/users/user_card_new';
import FieldsSetting from '../containers/fieldsSetting/list';
import { homePageSelector } from '../ducks/profile';

const MainRoute = withRouter(props => {
    const homePage = useSelector(state => homePageSelector(state));

    useEffect(() => {
        const { history, location } = props;
        const { pathname } = location;
        if (pathname === '/grid' && homePage) {
            history.push(homePage);
        }
    }, [homePage]);

    return (
        <Switch>
            <PrivateRoute
                exact
                path="/"
                component={() => <Redirect to={homePage} />}
            />
            <PrivateRoute exact path="/grid/:name" component={CustomGrid}/>
            <PrivateRoute exact path={DICTIONARY_NEW_LINK} component={CustomDictionaryCard}/>
            <PrivateRoute exact path={DICTIONARY_CARD_LINK} component={CustomDictionaryCard}/>
            <PrivateRoute exact path={DICTIONARY_LIST_LINK} component={CustomDictionaryList}/>
            <PrivateRoute exact path={ROLES_LINK} permission="editRoles" component={RolesList}/>
            <PrivateRoute exact path={NEW_USER_LINK} permission="editUsers" component={UserCard}/>
            <PrivateRoute exact path={USER_LINK} permission="editUsers" component={UserCard}/>
            <PrivateRoute exact path={USERS_LINK} permission="editUsers" component={UsersList}/>
            <PrivateRoute exact path={FIELDS_SETTING_LINK} permission="editFieldProperties" component={FieldsSetting}/>
            <Route exact path={LOGIN_LINK} component={Login} />
            <PrivateRoute exact path="*" component={() => <Redirect to={homePage}/>}/>
        </Switch>
    );
});

export default MainRoute;
