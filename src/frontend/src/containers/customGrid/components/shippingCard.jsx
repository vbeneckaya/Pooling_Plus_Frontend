import React, {useCallback, useEffect, useMemo, useState} from 'react';
import {useTranslation} from 'react-i18next';
import Information from './shippingTabs/information';
import Routes from './shippingTabs/routes';
import Documents from './shared/documents';
import History from './shared/history';
import Accounts from './shippingTabs/accounts';
import {useDispatch, useSelector} from 'react-redux';
import {userPermissionsSelector, userActionsSelector} from '../../../ducks/profile';
import {actionsCardSelector, invokeActionRequest} from '../../../ducks/gridActions';
import {cardSelector, editCardRequest, errorSelector, getCardRequest} from '../../../ducks/gridCard';
import {
    editInnerCardRequest,
    errorInnerSelector,
    getInerCardRequest,
    innerCardSelector
} from '../../../ducks/gridInnerCard';
import CardLayout from '../../../components/CardLayout';
import Orders from "./shippingTabs/orders";
import {Button, Confirm, Form, Grid, Modal, Segment} from "semantic-ui-react";
import FormField from "../../../components/BaseComponents";
import {ordersMiniColumns} from "../../../constants/ordersMiniColumns";
import {DATE_TYPE, NUMBER_TYPE, SELECT_TYPE, TEXT_TYPE} from "../../../constants/columnTypes";
import List from '../../../containers/customSelectGrid/list';
//import List from '../../../containers/customGrid/list';
import {
    // editCardRequest,
    settingsFormExtSelector,
} from "../../../ducks/gridCard";
import {isUniqueNumberInnerRequest} from "../../../ducks/gridInnerCard";
import {ORDERS_GRID} from "../../../constants/grids";

const Content = ({
                     t,
                     error,
                     form,
                     onChange,
                     onBlur,
                     uniquenessNumberInnerCheck,
                 }) => {
    const extSearchParamsFromDeliveryWarehouse = useMemo(() => ({
        clientId: form['clientId'] ? form['clientId'].value : undefined,
    }), [form['clientId']]);

    const settings = useSelector(state => settingsFormExtSelector(state, form.status));

    return (
        <Form className="tabs-card">
            <Grid>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <FormField
                            name="clientId"
                            type={SELECT_TYPE}
                            source="clients"
                            isRequired
                            settings={settings['clientId']}
                            error={error['clientId']}
                            value={form['clientId']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="orderNumber"
                            type={TEXT_TYPE}
                            isRequired
                            value={!!form['orderNumber'] ? form['orderNumber'].value : form['orderNumber']}
                            settings={settings['orderNumber']}
                            error={error['orderNumber']}
                            onBlur={uniquenessNumberInnerCheck}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="clientOrderNumber"
                            type={TEXT_TYPE}
                            settings={settings['clientOrderNumber']}
                            error={error["clientOrderNumber"]}
                            value={form['clientOrderNumber']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <FormField
                            name="palletsCount"
                            type={NUMBER_TYPE}
                            settings={settings['palletsCount']}
                            source="palletsCount"
                            value={form['palletsCount']}
                            error={error['palletsCount']}
                            rows={2}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="orderAmountExcludingVAT"
                            type={NUMBER_TYPE}
                            settings={settings['orderAmountExcludingVAT']}
                            value={form['orderAmountExcludingVAT']}
                            error={error['orderAmountExcludingVAT']}
                            rows={2}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="weightKg"
                            type={NUMBER_TYPE}
                            settings={settings['weightKg']}
                            source="weightKg"
                            value={form['weightKg']}
                            error={error['weightKg']}
                            rows={2}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('route')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <FormField
                                                name="shippingWarehouseId"
                                                type={SELECT_TYPE}
                                                settings={settings['shippingWarehouseId']}
                                                source="shippingWarehouses"
                                                isRequired
                                                value={form['shippingWarehouseId']}
                                                error={error['shippingWarehouseId']}
                                                rows={2}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="deliveryWarehouseId"
                                                type={SELECT_TYPE}
                                                settings={settings['deliveryWarehouseId']}
                                                isDisabled={!form['clientId']}
                                                isRequired
                                                extSearchParams={extSearchParamsFromDeliveryWarehouse}
                                                source="warehouses/byClientId"
                                                value={form['deliveryWarehouseId']}
                                                error={error['deliveryWarehouseId']}
                                                rows={2}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <FormField
                                                name="shippingDate"
                                                type={DATE_TYPE}
                                                settings={settings['shippingDate']}
                                                isRequired
                                                value={form['shippingDate']}
                                                error={error['shippingDate']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="deliveryDate"
                                                type={DATE_TYPE}
                                                settings={settings['deliveryDate']}
                                                isRequired
                                                value={form['deliveryDate']}
                                                error={error['deliveryDate']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        </Form>
    );
};

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
            actionsFooter,
            actionsHeader,
            loading,
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
        const errorInner = useSelector(state => errorInnerSelector(state));

        const canOnionOrders = !!userActions.find(_ => _ == 'unionOrders');
        const canOnionOrdersInExisted = !!userActions.find(_ => _ == 'unionOrdersInExisted');
        const canEditOrders = !!userActions.find(_ => _ == 'unionOrdersInExisted');
        // const orderActions = useSelector(state => actionsCardSelector(state))

        let [routeActiveIndex, setRouteActiveIndex] = useState(0);

        let [orderForm, setOrderForm] = useState([]);
        let [orderCard, setOrderCard] = useState([]);
        let [selectedOrders, setSelectedOrders] = useState([]);
        let [isNewOrder, setIsNewOrder] = useState(true);

        let [notChangeOrderForm, setNotChangeOrderForm] = useState(true);
        let [confirmation, setConfirmation] = useState({open: false});

        let [showModal, setShowModal] = useState(false);
        let [showSelectModal, setShowSelectModal] = useState(false);

        const handleTabChange = useCallback((e, {activeIndex}) => {
            setRouteActiveIndex(activeIndex);
        }, []);


        useEffect(
            () => {
                if (notChangeOrderForm) {
                    Object.keys(orderForm).forEach(key => {
                        if (orderForm[key] !== orderCard[key]) {
                            setNotChangeOrderForm(false);
                        }
                    });
                }
            },
            [orderForm],
        );

        const handleCreateOrder = () => {
            let defaultOrderForm = {};
            orderColumns.forEach(column => {
                defaultOrderForm[column.name] = undefined
            });
            defaultOrderForm['id'] = undefined;
            setOrderCard(defaultOrderForm);
            setOrderForm(defaultOrderForm);
            setNotChangeOrderForm(true);
            setShowModal(true);
            setIsNewOrder(true);
        };

        const handleSelectOrder = () => {
            propsForSelect.name = ORDERS_GRID;
            propsForSelect.match.params.name = ORDERS_GRID;
            setShowSelectModal(true);
        };

        const handleUniquenessInnerCheck = callbackFunc => {
            (!id || orderForm.orderNumber !== orderCard.orderNumber) && dispatch(
                isUniqueNumberInnerRequest({
                    number: orderForm.orderNumber && orderForm.orderNumber.value,
                    fieldName: 'orderNumber',
                    errorText: t('number_already_exists'),
                    callbackSuccess: callbackFunc,
                }),
            );

        };

        const getPanes = () => {
            const obj = [
                {
                    menuItem: t('information'),
                    render: () => (
                        <Information form={form} onChange={onChangeForm} onBlur={onBlurForm} settings={settings}/>
                    ),
                },
                {
                    menuItem: t('orders'),
                    isCreateBtn: (!!(form && form.id) ? canOnionOrdersInExisted : canOnionOrders) && userPermissions.includes(2),
                    isSelectBtn: !!(form && form.id) ? canOnionOrdersInExisted : canOnionOrders,
                    createAction: handleCreateOrder,
                    selectAction: handleSelectOrder,
                    render: () => <Orders
                        form={form}
                        columns={orderColumns}
                        isDeleteBtn={!!(form && form.id) ? canOnionOrdersInExisted : canOnionOrders}
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
            dispatch(getInerCardRequest({
                name: ORDERS_GRID, id: form.orders[index].id, callbackSuccess: (result) => {
                    setOrderCard(result);
                    setOrderForm(result);
                    setNotChangeOrderForm(true);
                    setIsNewOrder(false);
                    setShowModal(true);
                }
            }))
        }

        const onCloseModal = () => {
            setShowModal(false);
            closeConfirmation();
        };

        const onCloseSelectModal = () => {
            setShowSelectModal(false);
            closeConfirmation();
        };


        const onOpenModal = () => {
        };

        const onOpenSelectModal = () => {
        };

        const onChangeOrderForm = useCallback((e, {name, value}) => {
            switch (name) {
                case 'orderNumber':
                    setOrderForm(prevState => ({
                        ...prevState,
                        [name]: {value: value, name: null}
                    }));
                    break;
                case 'clientId':
                    setOrderForm(prevState => ({
                        ...prevState,
                        [name]: value,
                        ['deliveryWarehouseId']: null
                    }));
                    break;
                default:
                    setOrderForm(prevState => ({
                        ...prevState,
                        [name]: value,
                    }));
            }

        }, []);

        const getOrderActionsFooter = useCallback(
            (isNew) => {
                return (
                    <>
                        <Button color="grey" onClick={handleClose}>
                            {t('CancelButton')}
                        </Button>
                        <Button
                            color="blue"
                            disabled={notChangeOrderForm}
                            onClick={handleSave}
                        >
                            {isNew ? t('AddButton') : t('SaveButton')}
                        </Button>
                    </>
                );
            },
            [orderForm, notChangeOrderForm],
        );

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

        const handleSave = () => {
            if (isNewOrder) {
                if (orderForm.orderNumber != orderCard.orderNumber)
                    handleUniquenessInnerCheck(() => {
                        invokeAction('insertNewOrder')
                    });
                else
                    invokeAction('insertNewOrder');
            }
            else {
                invokeAction('updateOrder');
            }
        };

        const handleSelect = (ids) => {
            setSelectedOrders(Array.from(ids));
        };

        const handleSelectModalSave = () => {
            invokeAction('insertExistedOrder');
        }

        const handleDelete = (id) => {
            invokeAction('delete', id);
        };

        const handleClose = () => {
            if (notChangeOrderForm) {
                onCloseModal();
            } else {
                setConfirmation({
                    open: true,
                    content: t('confirm_close_dictionary'),
                    onCancel: closeConfirmation(),
                    onConfirm: onCloseModal,
                });
            }
        };

        const handleSelectModalClose = () => {
            if (notChangeOrderForm) {
                onCloseSelectModal();
            } else {
                setConfirmation({
                    open: true,
                    content: t('confirm_close_dictionary'),
                    onCancel: closeConfirmation(),
                    onConfirm: onCloseModal,
                });
            }
        };

        const closeConfirmation = () => {
            setConfirmation({
                open: false,
            });
        };

        const invokeAction = (actionName, orderId) => {
            if (actionName == 'insertNewOrder') {
                dispatch(editInnerCardRequest({
                    name: ORDERS_GRID,
                    params: {
                        ...orderForm,
                        orderNumber: {value: orderForm.orderNumber && orderForm.orderNumber.value},
                        shippingId: id
                    },
                    callbackSuccess: (result) => {
                        let orderAction = 'unionOrdersInExisted';
                        dispatch(invokeActionRequest({
                            name: ORDERS_GRID,
                            actionName: orderAction,
                            ids: [result.id],
                            callbackSuccess: () => {
                                load();
                                onCloseModal();
                            }
                        }));
                    }
                }));
            }

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
                let orderAction = 'removeFromShipping';
                dispatch(invokeActionRequest({
                    name: ORDERS_GRID,
                    actionName: orderAction,
                    ids: [orderId],
                    callbackSuccess: () => {
                        load();
                    }
                }));
            }

            if (actionName == 'updateOrder') {
                dispatch(editInnerCardRequest({
                    name: ORDERS_GRID, params: orderForm, callbackSuccess: () => {
                        load();
                        onCloseModal();
                    }
                }));
            }
        };

        return (
            <>
                <CardLayout
                    title={title}
                    // actionsFooter={actionsFooter}
                    actionsHeader={actionsHeader}
                    content={getPanes}
                    onClose={onClose}
                    loading={loading}
                />
                <Modal
                    dimmer="blurring"
                    open={showModal}
                    closeOnDimmerClick={false}
                    onOpen={onOpenModal}
                    onClose={onCloseModal}
                    closeIcon
                >
                    <Modal.Header>{t('order')}</Modal.Header>
                    <Modal.Description>
                        {/*<Loader size="huge" active={loading}>
                            Loading
                        </Loader>*/}
                        <Content
                            error={errorInner}
                            t={t}
                            form={orderForm}
                            onChange={onChangeOrderForm}
                            uniquenessNumberInnerCheck={handleUniquenessInnerCheck}
                        />
                    </Modal.Description>
                    <Modal.Actions>{getOrderActionsFooter(isNewOrder)}</Modal.Actions>
                </Modal>

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

                <Confirm
                    dimmer="blurring"
                    open={confirmation.open}
                    onCancel={confirmation.onCancel || closeConfirmation}
                    cancelButton={t('cancelConfirm')}
                    confirmButton={t('Yes')}
                    onConfirm={confirmation.onConfirm}
                    content={confirmation.content}
                />
            </>
        );
    }
;

export default ShippingCard;
