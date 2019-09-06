import React, {useState, useEffect} from 'react';
import {Menu, Dropdown} from 'semantic-ui-react';
import PropTypes from 'prop-types';
import {Link} from 'react-router-dom';
import {useDispatch, useSelector} from 'react-redux';
import {useTranslation} from 'react-i18next';
import {
    dictionariesMenuSelector,
    getUserProfile,
    gridsMenuSelector,
    roleSelector,
    userNameSelector,
} from '../../ducks/profile';
import useReactRouter from 'use-react-router';
import {isAuthSelector} from '../../ducks/login';
import './style.scss';
import {DICTIONARY_LINK, GRID_LINK, ROLES_LINK, USERS_LINK} from "../../router/links";

const Header = () => {
    const dispatch = useDispatch();
    const grids = useSelector(state => gridsMenuSelector(state));
    const dictionaries = useSelector(state => dictionariesMenuSelector(state));
    const userName = useSelector(state => userNameSelector(state));
    const userRole = useSelector(state => roleSelector(state));
    const isAuth = useSelector(state => isAuthSelector(state));
    const {t} = useTranslation();
    const { location } = useReactRouter();

    const getProfile = () => {
        if (!userName) {
            dispatch(getUserProfile());
        }
    };

    const logOut = () => {
    };

    useEffect(getProfile, []);

    console.log('location', location);

    let [activeItem, setActiveItem] = useState(location.pathname);

    useEffect(() => {setActiveItem(location.pathname)}, [location.pathname])

    return (
        <>
            {isAuth ? (
                <header>
                    <Menu pointing secondary fixed="top">
                        <Menu.Item>LOGO</Menu.Item>
                        {grids &&
                        grids.map(item => (
                            <Menu.Item
                                className="large"
                                key={item}
                                as={Link}
                                to={GRID_LINK.replace(':name', item)}
                                name={item}
                                active={activeItem.includes(item)}
                            >
                                {t(item)}
                            </Menu.Item>
                        ))}
                        <Menu.Item
                            className="large"
                            key="roles"
                            as={Link}
                            to={ROLES_LINK}
                            active={activeItem.includes("roles")}
                            name="roles"
                        >
                            {t('roles')}
                        </Menu.Item>
                        <Menu.Item
                            className="large"
                            key="users"
                            as={Link}
                            to={USERS_LINK}
                            active={activeItem.includes("users")}
                            name="users"
                        >
                            {t('users')}
                        </Menu.Item>
                        {dictionaries && (
                            <Menu.Menu>
                                <Dropdown text={t('dictionaries')} item className={`${dictionaries.some(x => activeItem.includes(x)) && "superActive"}`}>
                                    <Dropdown.Menu>
                                        {dictionaries.map(item => {
                                            return (
                                                <Dropdown.Item
                                                    key={item}
                                                    as={Link}
                                                    to={DICTIONARY_LINK.replace(':name', item)}
                                                    active={activeItem.includes(item)}
                                                    name={item}
                                                >
                                                    {t(item)}
                                                </Dropdown.Item>
                                            );
                                        })}
                                    </Dropdown.Menu>
                                </Dropdown>
                            </Menu.Menu>
                        )}
                        <Menu.Menu position="right">
                            <Dropdown text={`${userName} (${t(userRole)})`} item>
                                <Dropdown.Menu>
                                    <Dropdown.Item onClick={logOut}>{t('exit')}</Dropdown.Item>
                                </Dropdown.Menu>
                            </Dropdown>
                        </Menu.Menu>
                    </Menu>
                </header>
            ) : null}
        </>
    );
};
Header.propTypes = {};

export default Header;
