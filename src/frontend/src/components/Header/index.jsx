import React, { useEffect, useState } from 'react';
import { Dropdown, Icon, Menu } from 'semantic-ui-react';
import { Link } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { useTranslation } from 'react-i18next';
import {
    dictionariesHeaderSelector,
    dictionariesMenuSelector,
    gridsMenuSelector,
    otherMenuSelector,
    rolesAndUsersMenu,
    roleSelector,
    userNameSelector,
} from '../../ducks/profile';
import useReactRouter from 'use-react-router';
import { isAuthSelector, logoutRequest } from '../../ducks/login';
import './style.scss';
import { DICTIONARY_LINK, GRID_LINK } from '../../router/links';

const Header = () => {
    const dispatch = useDispatch();
    const grids = useSelector(state => gridsMenuSelector(state));
    const dictionariesList = useSelector(state => dictionariesMenuSelector(state));
    const dictionariesMenu = useSelector(state => dictionariesHeaderSelector(state));
    const otherMenu = useSelector(state => otherMenuSelector(state));
    const usersAndRoles = useSelector(state => rolesAndUsersMenu(state));
    const userName = useSelector(state => userNameSelector(state));
    const userRole = useSelector(state => roleSelector(state));
    const isAuth = useSelector(state => isAuthSelector(state));
    const { t } = useTranslation();
    const { location } = useReactRouter();

    const logOut = () => {
        dispatch(logoutRequest());
    };

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
                        {dictionariesMenu && dictionariesMenu.length
                            ? dictionariesMenu.map(item => (
                                  <Menu.Item
                                      className="large"
                                      key={item}
                                      as={Link}
                                      to={DICTIONARY_LINK.replace(':name', item)}
                                      name={item}
                                      active={activeItem.includes(item)}
                                  >
                                      {t(item)}
                                  </Menu.Item>
                              ))
                            : null}
                        {otherMenu && otherMenu.length
                            ? otherMenu.map(item => (
                                  <Menu.Item
                                      className="large"
                                      key={item.name}
                                      as={Link}
                                      to={item.link}
                                      name={item.name}
                                      active={activeItem.includes(item.name)}
                                  >
                                      {t(item.name)}
                                  </Menu.Item>
                              ))
                            : null}
                        {(dictionariesList && dictionariesList.length) || usersAndRoles.length ? (
                            <Menu.Menu>
                                <Dropdown
                                    text={t('dictionaries')}
                                    item
                                    className={`${[
                                        ...dictionariesList,
                                        ...usersAndRoles.map(item => item.name),
                                    ].some(x => activeItem.includes(x)) && 'superActive'}`}
                                >
                                    <Dropdown.Menu>
                                        {dictionariesList.map(item => {
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
                                        {usersAndRoles.map(item => (
                                            <Dropdown.Item
                                                key={item.name}
                                                as={Link}
                                                to={item.link}
                                                active={activeItem.includes(item.name)}
                                                name={item.name}
                                            >
                                                {t(item.name)}
                                            </Dropdown.Item>
                                        ))}
                                    </Dropdown.Menu>
                                </Dropdown>
                            </Menu.Menu>
                        ) : null}
                        <div className="header-support">
                            <Icon name="question circle" />
                            <div className="header-support_contacts">
                                <a href="mailto:support@artlogics.ru">support@artlogics.ru</a>
                                <div>{t('support_work_time')}</div>
                            </div>
                            {userName && userRole ? (
                                <Menu.Menu>
                                    <Dropdown text={`${userName} (${t(userRole)})`} item>
                                        <Dropdown.Menu>
                                            <Dropdown.Item onClick={logOut}>
                                                {t('exit')}
                                            </Dropdown.Item>
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </Menu.Menu>
                            ) : null}
                        </div>
                    </Menu>
                </header>
            ) : null}
        </>
    );
};
Header.propTypes = {};

export default Header;
