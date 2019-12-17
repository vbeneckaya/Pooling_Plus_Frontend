import React from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { Tab } from 'semantic-ui-react';
import Information from './orderTabs/information';
import Position from './orderTabs/position';
import Returns from './orderTabs/returns';
import Documents from './shared/documents';
import History from './shared/history';
import CreateOrder from './orderTabs/createOrder';

import CardLayout from '../../../components/CardLayout';
import { userPermissionsSelector } from '../../../ducks/profile';

const OrderCard = ({
    form,
    onChangeForm,
    name,
    id,
    load,
    isNotUniqueNumber,
    uniquenessNumberCheck,
    settings,
    error,
                       title,
                       onClose,
                       actionsFooter,
}) => {
    const userPermissions = useSelector(state => userPermissionsSelector(state));

    const getPanes = () => {
        let obj = [
            {
                menuItem: 'information',
                render: () => (
                    <Information
                        form={form}
                        settings={settings}
                        error={error}
                        load={load}
                        isNotUniqueNumber={isNotUniqueNumber}
                        uniquenessNumberCheck={uniquenessNumberCheck}
                        onChange={onChangeForm}
                    />
                ),
            },
            {
                menuItem: 'position',
                render: () => (
                    <Position
                        form={form}
                        onChange={onChangeForm}
                        gridName={name}
                        load={load}
                        error={error}
                        settings={settings}
                    />
                ),
            },
            {
                menuItem: 'returns',
                render: () => (
                    <Returns
                        form={form}
                        settings={settings}
                        error={error}
                        onChange={onChangeForm}
                    />
                ),
            },
        ];

        if (userPermissions.includes(4) || userPermissions.includes(5)) {
            obj.push({
                menuItem: 'documents',
                render: () => (
                    <Documents
                        gridName={name}
                        cardId={id}
                        isEditPermissions={userPermissions.includes(5)}
                    />
                ),
            });
        }

        if (userPermissions.includes(6)) {
            obj.push({
                menuItem: 'history',
                render: () => <History cardId={id} status={form.status} />,
            });
        }

        return obj;
    };

    return (
        <>
            {form.id ? (
                <CardLayout
                    title={title}
                    actionsFooter={actionsFooter}
                    content={getPanes}
                    onClose={onClose}
                    loading={false}
                />
            ) : (
                <CardLayout
                    title={title}
                    actionsFooter={actionsFooter}
                    onClose={onClose}
                    loading={false}
                >
                    <CreateOrder
                        form={form}
                        error={error}
                        isNotUniqueNumber={isNotUniqueNumber}
                        uniquenessNumberCheck={uniquenessNumberCheck}
                        onChange={onChangeForm}
                    />
                </CardLayout>
            )}
        </>
    );
};

export default OrderCard;
