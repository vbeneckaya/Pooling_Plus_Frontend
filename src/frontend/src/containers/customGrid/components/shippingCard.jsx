import React, {useCallback, useEffect, useMemo, useState} from 'react';
import {Button, Confirm, Form, Grid, Modal, Segment} from "semantic-ui-react";
import {useTranslation} from 'react-i18next';
import Information from './shippingTabs/information';
import Routes from './shippingTabs/routes';
import Documents from './shared/documents';
import History from './shared/history';
import Accounts from './shippingTabs/accounts';
import {useDispatch, useSelector} from 'react-redux';
import {userPermissionsSelector, userActionsSelector} from '../../../ducks/profile';
import {invokeActionRequest} from '../../../ducks/gridActions';
import {editCardRequest, getCardRequest, settingsFormExtSelector} from '../../../ducks/gridCard';
import CardLayout from '../../../components/CardLayout';
import FormField from "../../../components/BaseComponents";
import Orders from "./shippingTabs/orders";
import {ordersMiniColumns} from "../../../constants/ordersMiniColumns";
import List from '../../../containers/customSelectGrid/list';
import {ORDERS_GRID} from "../../../constants/grids";
import {BIG_TEXT_TYPE, DATE_TYPE, NUMBER_TYPE, SELECT_TYPE, TEXT_TYPE} from "../../../constants/columnTypes";

import {
    editInnerCardRequest,
    errorInnerSelector,
    getInnerCardRequest,
    innerCardSelector,
    isUniqueNumberInnerRequest,
} from '../../../ducks/gridInnerCard';

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
                            onBlur={onBlur}
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
                            onBlur={onBlur}
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
                            onBlur={onBlur}
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
                            onBlur={onBlur}
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
            actionsHeader,
            loading,
            goToCard,
            load,
            update,
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
        const errorInner = useSelector(state => errorInnerSelector(state));
        const orderCard = useSelector(state => innerCardSelector(state));

        let [routeActiveIndex, setRouteActiveIndex] = useState(0);

        let [orderForm, setOrderForm] = useState([]);
        //  let [orderCard, setOrderCard] = useState([]);
        let [selectedOrders, setSelectedOrders] = useState([]);
        let [allowOrderToSend, setAllowOrderToSend] = useState(false);

        //   let [notChangeOrderForm, setNotChangeOrderForm] = useState(true);
        let [confirmation, setConfirmation] = useState({open: false});

        let [showCreateModal, setShowCreateModal] = useState(false);
        let [showSelectModal, setShowSelectModal] = useState(false);


        const handleTabChange = useCallback((e, {activeIndex}) => {
            setRouteActiveIndex(activeIndex);
        }, []);


        useEffect(
            () => {
                if (allowOrderToSend) {
                    handleSaveOrder();
                }
            },
            [orderForm],
        );

        const handleCreateOrder = () => {
            let defaultOrderForm = {};
            orderColumns.forEach(column => {
                defaultOrderForm[column.name] = null
            });
            defaultOrderForm['id'] = null;
            defaultOrderForm['shippingId'] = id;
            dispatch(editCardRequest({
                    name: ORDERS_GRID,
                    params: defaultOrderForm,
                    callbackSuccess: (result) => {
                        let orderAction = 'unionOrders';
                        dispatch(invokeActionRequest({
                            name: ORDERS_GRID,
                            actionName: orderAction,
                            ids: [result.id],
                            callbackSuccess: () => {
                                loadInnerCard(
                                    result.id, 
                                    ()=>{update(); setShowCreateModal(true); }
                                    )
                                //goToCard(ORDERS_GRID, result.id);
                            }
                        }));
                    }
                })
            );
        };

        const loadInnerCard = (orderId, callbackFunc) => {
            dispatch(getInnerCardRequest({
                name: ORDERS_GRID,
                id: orderId,
                callbackSuccess: (result) => {
                    setAllowOrderToSend(false);
                    setOrderForm(result);
                    callbackFunc && callbackFunc();
                }
            }));
        }

        const onBlurOrderForm = () => {
            handleSaveOrder();
        };
        const saveOrEditOrderForm = () => {
            dispatch(
                editInnerCardRequest({
                    name: ORDERS_GRID,
                    params: orderForm,
                    callbackSuccess: (result) => {
                        loadInnerCard(result.id, update);
                    }
                }),
            );
        };

        const handleSaveOrder = () => {
            handleUniquenessInnerCheck(saveOrEditOrderForm);
        };

        const handleSelectOrder = () => {
            propsForSelect.name = ORDERS_GRID;
            propsForSelect.match.params.name = ORDERS_GRID;
            setShowSelectModal(true);
        };

        const handleUniquenessInnerCheck = callbackFunc => {
            if (orderForm)
                if (orderForm.orderNumber && orderCard.orderNumber && orderForm.orderNumber.value !== orderCard.orderNumber.value) {
                    dispatch(
                        isUniqueNumberInnerRequest({
                            number: orderForm.orderNumber && orderForm.orderNumber.value,
                            fieldName: 'orderNumber',
                            errorText: t('number_already_exists'),
                            callbackSuccess: callbackFunc,
                        }),
                    );
                }
                else {
                    callbackFunc();
                }
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

            let column = orderColumns.filter(_ => _.name == name)[0];
            let columnType = name == 'orderNumber' ? TEXT_TYPE : column && column.type;

            switch (columnType) {
                case TEXT_TYPE:
                case BIG_TEXT_TYPE:
                case NUMBER_TYPE:
                    setAllowOrderToSend(false);
                    break;
                default:
                    setAllowOrderToSend(true);
            }
        }, []);


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
                    isCreateBtn: (!!(form && form.id) ? canUnionOrdersInExisted : canUnionOrders) && userPermissions.includes(2),
                    isSelectBtn: false, //!!(form && form.id) ? canOnionOrdersInExisted : canOnionOrders,
                    createAction: handleCreateOrder,
                    selectAction: handleSelectOrder,
                    render: () => <Orders
                        form={form}
                        columns={orderColumns}
                        loadCard={update}
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
            debugger;
            dispatch(getCardRequest({
                name: ORDERS_GRID, id: form.orders[index].id, callbackSuccess: (result) => {
                    goToCard(ORDERS_GRID, result.id);
                }
            }))
        }

        const onCloseCreateModal = () => {
            setShowCreateModal(false);
            //closeConfirmation();
        };

        const onCloseSelectModal = () => {
            setShowSelectModal(false);
        };

        const onOpenSelectModal = () => {
        };

        const onOpenCreateModal = () => {
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
                        onCloseSelectModal();
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
                                update();
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

                {/*Modal for create*/}
                <Modal
                    dimmer="blurring"
                    open={showCreateModal}
                    closeOnDimmerClick={false}
                    onOpen={onOpenCreateModal}
                    onClose={onCloseCreateModal}
                    closeIcon
                >
                    <Modal.Header>{t('create_orders')}</Modal.Header>
                    <Modal.Description>
                        {/*<List*/}
                        {/*props={propsForSelect}*/}
                        {/*selectCallback={handleSelect}*/}
                        {/*action={actionAddToShipping}*/}
                        {/*>*/}
                        {/*</List>*/}
                        <Content
                            error={errorInner}
                            t={t}
                            form={orderForm}
                            onChange={onChangeOrderForm}
                            onBlur={onBlurOrderForm}
                            uniquenessNumberInnerCheck={handleSaveOrder}
                        />
                    </Modal.Description>
                </Modal>


                {/*List for select*/}
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
