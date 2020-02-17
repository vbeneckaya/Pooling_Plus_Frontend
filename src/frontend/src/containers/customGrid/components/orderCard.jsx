import React from 'react';
import { useSelector } from 'react-redux';
import Information from './orderTabs/information';
import Position from './orderTabs/position';
import Documents from './shared/documents';
import History from './shared/history';
import CreateOrder from './orderTabs/createOrder';

import CardLayout from '../../../components/CardLayout';
import { userPermissionsSelector } from '../../../ducks/profile';
import {ORDERS_GRID} from "../../../constants/grids";
import {columnsGridSelector} from "../../../ducks/gridList";

const OrderCard = (props) => {
    const {
        form,
        onChangeForm,
        onBlurForm,
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
        actionsHeader,
        loading,
    } = props;
    const userPermissions = useSelector(state => userPermissionsSelector(state));
    const columns = useSelector(state => columnsGridSelector(state, ORDERS_GRID));
    
    const getPanes = () => {
        let obj = [
            {
                menuItem: 'information',
                render: () => (
                    <Information
                        form={form}
                        columns={columns}
                        settings={settings}
                        error={error}
                        load={load}
                        isNotUniqueNumber={isNotUniqueNumber}
                        uniquenessNumberCheck={uniquenessNumberCheck}
                        onChange={onChangeForm}
                        onBlur={onBlurForm}
                    />
                ),
            },
            /*{
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
            },*/
        ];
        /*{
                menuItem: 'returns',
                render: () => (
                    <Returns
                        form={form}
                        settings={settings}
                        error={error}
                        onChange={onChangeForm}
                    />
                ),
            },*/

        if (userPermissions.includes(4) || userPermissions.includes(5)) {
            obj.push({
                menuItem: 'documents',
                render: () => (
                    <Documents
                        gridName={name}
                        cardId={id}
                        load={load}
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
            {id ? (
                <CardLayout
                    title={title}
                    //     actionsFooter={actionsFooter}
                    actionsHeader={actionsHeader}
                    content={getPanes}
                    onClose={onClose}
                    loading={loading}
                />
            ) : (
                <CardLayout
                    title={title}
                    actionsHeader={actionsHeader}
                    // actionsFooter={actionsFooter}
                    onClose={onClose}
                    loading={loading}
                    connectToShipping={false}
                >
                    <CreateOrder
                        form={form}
                        error={error}
                       // isNotUniqueNumber={isNotUniqueNumber}
                        uniquenessNumberCheck={uniquenessNumberCheck}
                        onChange={onChangeForm}
                        onBlur={onBlurForm}
                    />
                </CardLayout>
            )}
        </>
    );
};

export default OrderCard;
