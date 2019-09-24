import React from 'react';
import {useTranslation} from 'react-i18next';

import {Tab} from 'semantic-ui-react';
import Information from "./shippingTabs/information";
import Route from "./shippingTabs/route";
import Documents from "./shippingTabs/documents";
import History from "./shippingTabs/history";
import Accounts from "./shippingTabs/accounts";
import AdditionalExpenses from "./shippingTabs/additionalExpenses";

const ShippingModal = ({form, onChangeForm}) => {
    const { t } = useTranslation();

    const getPanes = [
        {
            menuItem: t('information'),
            render: () => <Tab.Pane><Information form={form} onChange={onChangeForm}/></Tab.Pane>,
        },
        {
            menuItem: t('route'),
            render: () => <Tab.Pane><Route/></Tab.Pane>,
        },
        {
            menuItem: t('accounts'),
            render: () => <Tab.Pane><Accounts/></Tab.Pane>,
        },
        {
            menuItem: t('additionalExpenses'),
            render: () => <Tab.Pane><AdditionalExpenses/></Tab.Pane>,
        },
        {
            menuItem: t('documents'),
            render: () => <Tab.Pane><Documents/></Tab.Pane>,
        },
        {
            menuItem: t('history'),
            render: () => <Tab.Pane><History/></Tab.Pane>,
        },
    ];

    return <Tab panes={getPanes} />;
};

export default ShippingModal;
