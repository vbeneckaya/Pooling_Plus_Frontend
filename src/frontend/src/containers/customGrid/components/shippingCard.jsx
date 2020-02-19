import React, {useCallback, useEffect, useMemo, useState} from 'react';
import {useTranslation} from 'react-i18next';
import Information from './shippingTabs/information';
import Routes from './shippingTabs/routes';
import Documents from './shared/documents';
import History from './shared/history';
import Accounts from './shippingTabs/accounts';
import {useDispatch, useSelector} from 'react-redux';
import {userPermissionsSelector, userActionsSelector} from '../../../ducks/profile';
import {invokeActionRequest} from '../../../ducks/gridActions';
import {getCardRequest, setFormIntoCard} from '../../../ducks/gridCard';
import CardLayout from '../../../components/CardLayout';
import Orders from "./shippingTabs/orders";
import {Button, Modal} from "semantic-ui-react";
import {ordersMiniColumns} from "../../../constants/ordersMiniColumns";
import List from '../../../containers/customSelectGrid/list';
import {ORDERS_GRID} from "../../../constants/grids";
import {SHIPPINGS_GRID_AS_PARENT} from "../../../constants/parentGrids";

const ShippingCard = (props) => {
        const {
            form,
            onChangeForm,
            onBlurForm,
            name,
            id,
            onClose,
            settings,
            error,
            title,
            actionsHeader,
            loading,
            goToCard,
            load,
            history,
        } = props;

        let propsForSelect = {
            history: history,
            location: props.location,
            match: props.match,
            name: ORDERS_GRID,
        }


        const {t} = useTranslation();
        const dispatch = useDispatch();

        const orderColumns = ordersMiniColumns;
        const actionAddToShipping = {name: 'unionOrdersInExisted', buttonName: 'AddButton', shippingId: form.id};

        const userPermissions = useSelector(state => userPermissionsSelector(state));
        const userActions = useSelector(state => userActionsSelector(state));

        const canUnionOrders = !!userActions.find(_ => _ == 'unionOrders');
        const canUnionOrdersInExisted = !!userActions.find(_ => _ == 'unionOrdersInExisted');
        const canEditOrders = !!userActions.find(_ => _ == 'unionOrdersInExisted');

        let [routeActiveIndex, setRouteActiveIndex] = useState(0);

        let [selectedOrders, setSelectedOrders] = useState([]);

        let [showModal, setShowModal] = useState(false);
        let [showSelectModal, setShowSelectModal] = useState(false);

        const handleTabChange = useCallback((e, {activeIndex}) => {
            setRouteActiveIndex(activeIndex);
        }, []);

        const handleCreateOrder = () => {
            let defaultOrderForm = {};
            orderColumns.forEach(column => {
                defaultOrderForm[column.name] = null
            });
            defaultOrderForm['id'] = null;
            defaultOrderForm['shippingId'] = id;

            dispatch(setFormIntoCard({
                form: defaultOrderForm,
                callbackSuccess: () => {
                    goToCard(ORDERS_GRID, 'new', SHIPPINGS_GRID_AS_PARENT);
                }
            }));
        };

        const handleSelectOrder = () => {
            propsForSelect.name = ORDERS_GRID;
            propsForSelect.match.params.name = ORDERS_GRID;
            setShowSelectModal(true);
        };

        const getPanes = () => {
            const obj = [
                {
                    menuItem: t('information'),
                    render: () => (
                        <Information form={form} onChange={onChangeForm} onBlur={onBlurForm} settings={settings} />
                    ),
                },
                {
                    menuItem: t('orders'),
                    isCreateBtn: (!!(form && form.id) ? canUnionOrdersInExisted : canUnionOrders) && userPermissions.includes(2),
                    isSelectBtn: false, //!!(form && form.id) ? canOnionOrdersInExisted : canOnionOrders,
                    createAction: handleCreateOrder,
                    selectAction: handleSelectOrder,
                    render: () => <Orders
                        form={form}
                        columns={orderColumns}
                        isDeleteBtn={!!(form && form.id) ? canUnionOrdersInExisted : canUnionOrders}
                        removeFromShipping={handleDelete}
                        openOrderModal={openOrderModal}
                    />
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
                            onBlur={onBlurForm}
                        />
                    ),
                },

                {
                    menuItem: t('accounts'),
                    render: () => <Accounts form={form} settings={settings} onChange={onChangeForm} onBlur={onBlurForm}/>,
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

        const openOrderModal = (index) => {
            dispatch(getCardRequest({
                name: ORDERS_GRID, id: form.orders[index].id, callbackSuccess: (result) => {
                    goToCard(ORDERS_GRID, result.id);
                }
            }))
        }

        const onCloseModal = () => {
            setShowModal(false);
            //closeConfirmation();
        };

        const onCloseSelectModal = () => {
            setShowSelectModal(false);
        };

        const onOpenSelectModal = () => {
        };

        const getSElectOrderActionsFooter = useCallback(
            () => {
                return (
                    <>
                        <Button color="grey" onClick={handleSelectModalClose}>
                            {t('CancelButton')}
                        </Button>
                        <Button
                            color="blue"
                            //disabled={notChangeOrderForm}
                            onClick={handleSelectModalSave}
                        >
                            {t('AddButton')}
                        </Button>
                    </>
                );
            },
            [],
        );

        const handleSelect = (ids) => {
            setSelectedOrders(Array.from(ids));
        };

        const handleSelectModalSave = () => {
            invokeAction('insertExistedOrder');
        }

        const handleDelete = (id) => {
            invokeAction('delete', id);
        };

        const handleSelectModalClose = () => {
            onCloseSelectModal();
        };

        // const closeConfirmation = () => {
        //     setConfirmation({
        //         open: false,
        //     });
        // };

        const invokeAction = (actionName, orderId) => {
            if (actionName == 'insertExistedOrder') {
                let orderAction = 'unionOrdersInExisted';
                dispatch(invokeActionRequest({
                    name: ORDERS_GRID,
                    actionName: orderAction,
                    ids: selectedOrders,
                    callbackSuccess: () => {
                        load();
                        onCloseModal();
                    }
                }));
            }

            if (actionName == 'delete') {
                
                let orderAction1 = 'removeFromShipping';
                let orderAction2 = 'deleteOrder';
                
                dispatch(invokeActionRequest({
                    name: ORDERS_GRID,
                    actionName: orderAction1,
                    ids: [orderId],
                    callbackSuccess: () => {
                        dispatch(invokeActionRequest({
                            name: ORDERS_GRID,
                            actionName: orderAction2,
                            ids: [orderId],
                            callbackSuccess: () => {
                                load();
                            }
                        }));
                    }
                }));
            }
        };


        return (
            <>
                <CardLayout
                    title={title}
                    actionsHeader={actionsHeader}
                    content={getPanes}
                    onClose={onClose}
                    loading={loading}
                />

                <Modal
                    dimmer="blurring"
                    open={showSelectModal}
                    closeOnDimmerClick={false}
                    onOpen={onOpenSelectModal}
                    onClose={onCloseSelectModal}
                    size='fullscreen'
                    closeIcon
                >
                    <Modal.Header>{t('select_order')}</Modal.Header>
                    <Modal.Description>
                        <List
                            props={propsForSelect}
                            selectCallback={handleSelect}
                            action={actionAddToShipping}
                        >
                        </List>
                    </Modal.Description>
                    <Modal.Actions>{getSElectOrderActionsFooter()}</Modal.Actions>
                </Modal>

                {/*<Confirm*/}
                    {/*dimmer="blurring"*/}
                    {/*open={confirmation.open}*/}
                    {/*onCancel={confirmation.onCancel || closeConfirmation}*/}
                    {/*cancelButton={t('cancelConfirm')}*/}
                    {/*confirmButton={t('Yes')}*/}
                    {/*onConfirm={confirmation.onConfirm}*/}
                    {/*content={confirmation.content}*/}
                {/*/>*/}
            </>
        );
    }
;

export default ShippingCard;
