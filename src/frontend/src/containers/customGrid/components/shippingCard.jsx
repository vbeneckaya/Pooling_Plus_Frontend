import React, {useCallback, useState} from 'react';
import {useTranslation} from 'react-i18next';
import Information from './shippingTabs/information';
import Routes from './shippingTabs/routes';
import Documents from './shared/documents';
import History from './shared/history';
import Accounts from './shippingTabs/accounts';
import {useSelector} from 'react-redux';
import {userPermissionsSelector} from '../../../ducks/profile';
import CardLayout from '../../../components/CardLayout';

const ShippingCard = ({
                          form,
                          onChangeForm,
                          name,
                          id,
                          onClose: beforeClose,
                          settings,
                          title,
                          actionsFooter,
                          onClose,
                          actionsHeader,
                          loading,
                          load,
                      }) => {
    const {t} = useTranslation();
    const userPermissions = useSelector(state => userPermissionsSelector(state));
    const {orders = []} = form;
    let [routeActiveIndex, setRouteActiveIndex] = useState(0);

    const handleTabChange = useCallback((e, {activeIndex}) => {
        setRouteActiveIndex(activeIndex);
    }, []);

    const getPanes = () => {
        const obj = [
            {
                menuItem: t('information'),
                render: () => (
                    <Information form={form} onChange={onChangeForm} settings={settings}/>
                ),
            },
            {
                menuItem: t('route'),
                render: () => (
                    <Routes
                        form={form}
                        routeActiveIndex={routeActiveIndex}
                        settings={settings}
                        tabChange={handleTabChange}
                        onChange={onChangeForm}
                    />
                ),
            },
            {
                menuItem: t('accounts'),
                render: () => <Accounts form={form} settings={settings} onChange={onChangeForm}/>,
            },
        ];

        if (userPermissions.includes(10) || userPermissions.includes(11)) {
            obj.push({
                menuItem: t('documents'),
                render: () => (
                    <Documents
                        gridName={name}
                        cardId={id}
                        load={load}
                        isEditPermissions={userPermissions.includes(11)}
                    />
                ),
            });
        }

        if (userPermissions.includes(12)) {
            obj.push({
                menuItem: t('history'),
                render: () => <History cardId={id} status={form.status}/>,
            });
        }

        return obj;
    };

    return (
        <>
            {/*<div className="vertical-menu-card">
            <Menu vertical>
                <Menu.Item header>Заказы</Menu.Item>
                {orders.map(order => (
                    <Card
                        key={order.id}
                        name="orders"
                        id={order.id}
                        title={`edit_orders`}
                        onClose={beforeClose}
                    >
                        <Menu.Item>{t('order_item', {number: order.orderNumber})}</Menu.Item>
                    </Card>
                ))}
            </Menu>
            <div className="shipping-card-content">
                <Tab panes={getPanes}/>
            </div>
        </div>*/}
            <CardLayout
                title={title}
                actionsFooter={actionsFooter}
                actionsHeader={actionsHeader}
                content={getPanes}
                onClose={onClose}
                loading={loading}
            />
        </>
    );
};

export default ShippingCard;
