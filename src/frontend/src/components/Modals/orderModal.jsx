import React from 'react';
import { useTranslation } from 'react-i18next';

import { Tab } from 'semantic-ui-react';
import Information from './orderTabs/information';
import Position from './orderTabs/position';
import Returns from './orderTabs/returns';
import Documents from './shared/documents';
import History from './shared/history';
import CreateOrder from './orderTabs/createOrder';

const OrderModal = ({ form, onChangeForm, name, id, load, isNotUniqueNumber, uniquenessNumberCheck }) => {
    const { t } = useTranslation();

    const getPanes = [
        {
            menuItem: t('information'),
            render: () => (
                <Tab.Pane className="tabs-card">
                    <Information form={form} isNotUniqueNumber={isNotUniqueNumber} uniquenessNumberCheck={uniquenessNumberCheck} onChange={onChangeForm} />
                </Tab.Pane>
            ),
        },
        {
            menuItem: t('position'),
            render: () => (
                <Tab.Pane className="tabs-card">
                    <Position form={form} onChange={onChangeForm} gridName={name} load={load}/>
                </Tab.Pane>
            ),
        },
        {
            menuItem: t('returns'),
            render: () => (
                <Tab.Pane className="tabs-card">
                    <Returns form={form} onChange={onChangeForm} />
                </Tab.Pane>
            ),
        },
        {
            menuItem: t('documents'),
            render: () => (
                <Tab.Pane className="tabs-card">
                    <Documents gridName={name} cardId={id} />
                </Tab.Pane>
            ),
        },
        {
            menuItem: t('history'),
            render: () => (
                <Tab.Pane className="tabs-card">
                    <History cardId={id} status={form.status}/>
                </Tab.Pane>
            ),
        },
    ];

    return <>{form.id ? <Tab panes={getPanes} /> : <CreateOrder form={form} isNotUniqueNumber={isNotUniqueNumber} uniquenessNumberCheck={uniquenessNumberCheck} onChange={onChangeForm}/>}</>;
};

export default OrderModal;
