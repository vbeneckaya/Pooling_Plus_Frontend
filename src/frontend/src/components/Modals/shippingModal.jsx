import React from 'react';
import {useTranslation} from 'react-i18next';

import {Tab} from 'semantic-ui-react';
import Information from "./shippingTabs/information";
import Routes from "./shippingTabs/routes";
import Documents from "./shippingTabs/documents";
import History from "./shippingTabs/history";
import Accounts from "./shippingTabs/accounts";

const ShippingModal = ({form, onChangeForm}) => {
    const { t } = useTranslation();

    const getPanes = [
        {
            menuItem: t('information'),
            render: () => <Tab.Pane className='tabs-card'><Information form={form} onChange={onChangeForm} /></Tab.Pane>,
        },
        {
            menuItem: t('route'),
            render: () => <Tab.Pane className='tabs-card'><Routes form={form} onChange={onChangeForm} /></Tab.Pane>,
        },
        {
            menuItem: t('accounts'),
            render: () => <Tab.Pane className='tabs-card'><Accounts form={form} onChange={onChangeForm} /></Tab.Pane>,
        },
        {
            menuItem: t('documents'),
            render: () => <Tab.Pane className='tabs-card'><Documents/></Tab.Pane>,
        },
        {
            menuItem: t('history'),
            render: () => <Tab.Pane className='tabs-card'><History/></Tab.Pane>,
        },
    ];

    return <Tab panes={getPanes} />;
};

export default ShippingModal;
