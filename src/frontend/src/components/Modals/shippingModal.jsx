import React from 'react';
import {useTranslation} from 'react-i18next';

import {Grid, Menu, Tab} from 'semantic-ui-react';
import Information from './shippingTabs/information';
import Routes from './shippingTabs/routes';
import Documents from './shared/documents';
import History from './shippingTabs/history';
import Accounts from './shippingTabs/accounts';

const ShippingModal = ({form, onChangeForm, name, id}) => {
    const {t} = useTranslation();

    const getPanes = [
        {
            menuItem: t('information'),
            render: () => (
                <Tab.Pane className="tabs-card">
                    <Information form={form} onChange={onChangeForm}/>
                </Tab.Pane>
            ),
        },
        {
            menuItem: t('route'),
            render: () => (
                <Tab.Pane className="tabs-card">
                    <Routes form={form} onChange={onChangeForm}/>
                </Tab.Pane>
            ),
        },
        {
            menuItem: t('accounts'),
            render: () => (
                <Tab.Pane className="tabs-card">
                    <Accounts form={form} onChange={onChangeForm}/>
                </Tab.Pane>
            ),
        },
        {
            menuItem: t('documents'),
            render: () => (
                <Tab.Pane className="tabs-card">
                    <Documents gridName={name} cardId={id}/>
                </Tab.Pane>
            ),
        },
        {
            menuItem: t('history'),
            render: () => (
                <Tab.Pane className="tabs-card">
                    <History/>
                </Tab.Pane>
            ),
        },
    ];

    return (
        <div className="vertical-menu-card">
            <Menu vertical>
                <Menu.Item header>Заказы</Menu.Item>
                <Menu.Item>Заказ 1</Menu.Item>
                <Menu.Item>Заказ 2</Menu.Item>
                <Menu.Item>Заказ 3</Menu.Item>
            </Menu>
            <div className="shipping-card-content">
                <Tab panes={getPanes}/>
            </div>
        </div>
    );
};

export default ShippingModal;
