import React, { useState, useEffect } from 'react';
import { Menu, Dropdown } from 'semantic-ui-react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { useTranslation } from 'react-i18next';
import {
    dictionariesMenuSelector,
    getUserProfile,
    gridsMenuSelector,
    rolesAndUsersMenu,
    roleSelector,
    userNameSelector,
} from '../../ducks/profile';
import useReactRouter from 'use-react-router';
import { isAuthSelector, logoutRequest } from '../../ducks/login';
import './style.scss';
import { DICTIONARY_LINK, GRID_LINK, ROLES_LINK, USERS_LINK } from '../../router/links';

const Header = () => {
    const dispatch = useDispatch();
    const grids = useSelector(state => gridsMenuSelector(state));
    const dictionaries = useSelector(state => dictionariesMenuSelector(state));
    const usersAndRoles = useSelector(state => rolesAndUsersMenu(state));
    const userName = useSelector(state => userNameSelector(state));
    const userRole = useSelector(state => roleSelector(state));
    const isAuth = useSelector(state => isAuthSelector(state));
    const { t } = useTranslation();
    const { location } = useReactRouter();

    const getProfile = () => {
        if (!userName && isAuth) {
            dispatch(getUserProfile());
        }
    };

    const logOut = () => {
        dispatch(logoutRequest());
    };

    useEffect(getProfile, []);

    let [activeItem, setActiveItem] = useState(location.pathname);

    useEffect(
        () => {
            setActiveItem(location.pathname);
        },
        [location.pathname],
    );

    return (
        <>
            {isAuth ? (
                <header>
                    <Menu pointing secondary fixed="top" style={{ paddingLeft: '12px' }}>
                        {/*<Menu.Item>LOGO</Menu.Item>*/}
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
                        {usersAndRoles.map(item => (
                            <Menu.Item
                                className="large"
                                key={item.name}
                                as={Link}
                                to={item.link}
                                active={activeItem.includes(item.name)}
                                name="roles"
                            >
                                {t(item.name)}
                            </Menu.Item>
                        ))}
                        {dictionaries && (
                            <Menu.Menu>
                                <Dropdown
                                    text={t('dictionaries')}
                                    item
                                    className={`${dictionaries.some(x => activeItem.includes(x)) &&
                                        'superActive'}`}
                                >
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
                        {userName && userRole ? (
                            <Menu.Menu position="right">
                                <Dropdown text={`${userName} (${t(userRole)})`} item>
                                    <Dropdown.Menu>
                                        <Dropdown.Item onClick={logOut}>{t('exit')}</Dropdown.Item>
                                    </Dropdown.Menu>
                                </Dropdown>
                            </Menu.Menu>
                        ) : null}
                    </Menu>
                </header>
            ) : null}
        </>
    );
};
Header.propTypes = {};

export default Header;
