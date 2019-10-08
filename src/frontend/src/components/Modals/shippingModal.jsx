import React from 'react';
import {useTranslation} from 'react-i18next';

import {Grid, Menu, Tab} from 'semantic-ui-react';
import Information from './shippingTabs/information';
import Routes from './shippingTabs/routes';
import Documents from './shared/documents';
import History from './shippingTabs/history';
import Accounts from './shippingTabs/accounts';
import Card from "../../containers/customGrid/card";

const ShippingModal = ({form, onChangeForm, name, id, onClose}) => {
    const {t} = useTranslation();
    const {orders = []} = form;

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
                {
                    orders.map(order => (
                        <Card
                            key={order.id}
                            name="orders"
                            id={order.id}
                            title={t(`edit_orders`, {
                                number: order.orderNumber,
                                status: '',
                            })}
                            onOpen={onClose}
                        >
                            <Menu.Item>{t('order_item', {number: order.orderNumber})}</Menu.Item>
                        </Card>
                    ))
                }
            </Menu>
            <div className="shipping-card-content">
                <Tab panes={getPanes}/>
            </div>
        </div>
    );
};

export default ShippingModal;
