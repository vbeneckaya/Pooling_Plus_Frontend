import React, {useEffect} from 'react';
import {Switch, Redirect, Route} from 'react-router-dom';
import {withRouter} from 'react-router';
import {useDispatch, useSelector} from 'react-redux';
import {
    DICTIONARY_CARD_LINK,
    DICTIONARY_LIST_LINK,
    DICTIONARY_NEW_LINK,
    FIELDS_SETTING_LINK,
    GRID_CARD_LINK,
    GRID_GRID_CARD_LINK,
    GRID_LIST_LINK,
    GRID_NEW_LINK,
    LOGIN_LINK,
    SIGN_WITHOUT_LOGIN,
    NEW_ROLE_LINK,
    NEW_USER_LINK, REPORTS_LINK,
    ROLE_LINK,
    ROLES_LINK,
    USER_LINK,
    USERS_LINK,
} from './links';
import CustomGrid from '../containers/customGrid/list';
import CustomGridCard from '../containers/customGrid/card';
import CustomDictionaryList from '../containers/customDictionary/list';
import CustomDictionaryCard from '../containers/customDictionary/card_new';
import PrivateRoute from './privateRoute';
import Login from '../containers/login';
import RolesList from '../containers/roles/roles_list';
import RoleCard from '../containers/roles/role_card_new';
import UsersList from '../containers/users/users_list';
import UserCard from '../containers/users/user_card';
import FieldsSetting from '../containers/fieldsSetting/list';
import {homePageSelector} from '../ducks/profile';
import Report from "../containers/report";
import SignWithoutLogin from "../containers/signWithoutLogin";
import {showInstruction} from "../ducks/profile";

const MainRoute = withRouter(props => {

    const dispatch = useDispatch();

    const homePage = useSelector(state => homePageSelector(state));

    const {history, location} = props;

    const {pathname} = location;

    useEffect(
        () => {
            const {history, location} = props;
            const {pathname} = location;
            if (pathname === '/grid' && homePage) {
                history.push(homePage);
            }
        },
        [homePage],
    );

    useEffect(() => {
        const alreadyInLocalStorage = localStorage.getItem(pathname);
        if (!alreadyInLocalStorage) {
            const t = showInstruction(pathname);
            dispatch(t);
        }
    }, [pathname])

    return (
        <Switch>
            <PrivateRoute exact path="/" component={() => <Redirect to={homePage}/>}/>
            <PrivateRoute exact path={GRID_NEW_LINK} component={CustomGridCard}/>
            <PrivateRoute exact path={GRID_CARD_LINK} component={props => CustomGridCard(props)}/>
            <PrivateRoute exact path={GRID_GRID_CARD_LINK} component={props => CustomGridCard(props)}/>
            <PrivateRoute exact path={GRID_LIST_LINK} component={CustomGrid}/>
            <PrivateRoute exact path={DICTIONARY_NEW_LINK} component={CustomDictionaryCard}/>
            <PrivateRoute exact path={DICTIONARY_CARD_LINK} component={CustomDictionaryCard}/>
            <PrivateRoute exact path={DICTIONARY_LIST_LINK} component={CustomDictionaryList}/>
            <PrivateRoute exact path={NEW_ROLE_LINK} permission="editRoles" component={RoleCard}/>
            <PrivateRoute exact path={ROLE_LINK} permission="editRoles" component={RoleCard}/>
            <PrivateRoute exact path={ROLES_LINK} permission="editRoles" component={RolesList}/>
            <PrivateRoute exact path={NEW_USER_LINK} permission="editUsers" component={UserCard}/>
            <PrivateRoute exact path={USER_LINK} permission="editUsers" component={UserCard}/>
            <PrivateRoute exact path={USERS_LINK} permission="editUsers" component={UsersList}/>
            <PrivateRoute exact path={FIELDS_SETTING_LINK} permission="editFieldProperties" component={FieldsSetting}/>
            <PrivateRoute exact path={REPORTS_LINK} component={Report}/>
            <Route exact path={LOGIN_LINK} component={Login}/>
            <Route exact path={SIGN_WITHOUT_LOGIN} component={SignWithoutLogin}/>
            <PrivateRoute exact path="*" component={() => <Redirect to={homePage}/>}/>

        </Switch>
    );
});

export default MainRoute;
