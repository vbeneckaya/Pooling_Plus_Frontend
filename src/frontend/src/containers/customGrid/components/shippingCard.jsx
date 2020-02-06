import React, {useCallback, useEffect, useMemo, useState} from 'react';
import {useTranslation} from 'react-i18next';
import Information from './shippingTabs/information';
import Routes from './shippingTabs/routes';
import Documents from './shared/documents';
import History from './shared/history';
import Accounts from './shippingTabs/accounts';
import {useDispatch, useSelector} from 'react-redux';
import {userPermissionsSelector, userActionsSelector} from '../../../ducks/profile';
import CardLayout from '../../../components/CardLayout';
import Orders from "./shippingTabs/orders";
import {Button, Confirm, Form, Grid, Modal, Segment} from "semantic-ui-react";
import FormField from "../../../components/BaseComponents";
import {ordersMiniColumns} from "../../../constants/ordersMiniColumns";
import {DATE_TYPE, NUMBER_TYPE, SELECT_TYPE, TEXT_TYPE} from "../../../constants/columnTypes";
import {
    editCardRequest,
    isUniqueNumberRequest,
    settingsFormExtSelector,
} from "../../../ducks/gridCard";
import {fieldsSettingSelector} from "../../../ducks/fieldsSetting";
import {columnsGridSelector} from "../../../ducks/gridList";
import {ORDERS_GRID} from "../../../constants/grids";
import {SETTINGS_TYPE_HIDE} from "../../../constants/formTypes";
import {getFieldsSettingRequest} from '../../../ducks/fieldsSetting';

const Content = ({t, error, form, onChange, uniquenessNumberCheck, isNotUniqueNumber}) => {
    const extSearchParamsFromDeliveryWarehouse = useMemo(() => ({
        clientId: form['clientId'] ? form['clientId'].value : undefined,
    }), [form['clientId']]);

    const settings = useSelector(state => settingsFormExtSelector(state, form.status));
    console.log(settings);

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
                            value={form['orderNumber'] ? form['orderNumber'].value ? form['orderNumber'].value : form['orderNumber'] : form['orderNumber']}
                            settings={settings['orderNumber']}
                            error={(isNotUniqueNumber && t('number_already_exists')) || error['orderNumber']}
                            onBlur={uniquenessNumberCheck}
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

        const {t} = useTranslation();
        const dispatch = useDispatch();

        const orderColumns = ordersMiniColumns;

        const userPermissions = useSelector(state => userPermissionsSelector(state));
        const userActions = useSelector(state => userActionsSelector(state));

        const canOnionOrders = userActions.find(_ => _ == 'unionOrders');
        const canOnionOrdersInExisted = userActions.find(_ => _ == 'unionOrdersInExisted');
        const canEditOrders = userActions.find(_ => _ == 'unionOrdersInExisted');

        let [routeActiveIndex, setRouteActiveIndex] = useState(0);

        let [orderForm, setOrderForm] = useState([]);
        let [orderCard, setOrderCard] = useState([]);
        let [indexRow, setIndexRow] = useState();


        let [notChangeOrderForm, setNotChangeOrderForm] = useState(true);
        let [confirmation, setConfirmation] = useState({open: false});

        let [showModal, setShowModal] = useState(false);

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
            setIndexRow(null);
            let defaultOrderForm = {};
            defaultOrderForm['id'] = undefined;
            orderColumns.forEach(column => {
                defaultOrderForm[column.name] = undefined
            });
            setOrderCard(defaultOrderForm);
            setOrderForm(defaultOrderForm);
            setNotChangeOrderForm(true);
            setShowModal(true);
        };

        const handleUniquenessCheck = callbackFunc => {
            (!id || orderForm.orderNumber !== orderCard.orderNumber) && dispatch(
                isUniqueNumberRequest({
                    number: orderForm.orderNumber,
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
                    menuItem: t('orders'),
                    isCreateBtn: userPermissions.includes(2),
                    createAction: handleCreateOrder,
                    render: () => <Orders
                        form={form}
                        columns={orderColumns}
                        isEditBtn={!!form.id ? canOnionOrdersInExisted : canOnionOrders}
                        isDeleteBtn={!!form.id ? canOnionOrdersInExisted : canOnionOrders}
                        onChange={onChangeForm}
                        openOrderModal={openOrderModal}
                        />
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

        const openOrderModal = (index) => {
            setIndexRow(index);
            setOrderCard(form.orders[index]);
            setOrderForm(form.orders[index]);
            setNotChangeOrderForm(true);
            setShowModal(true);
            console.log(settings);
        }

        const onCloseModal = () => {
            setShowModal(false);
            closeConfirmation();
        };

        const onOpenModal = () => {
        };

        const onChangeOrderForm = useCallback((e, {name, value}) => {
            console.log('effectOrder');
            setOrderForm(prevState => ({
                ...prevState,
                [name]: value,
            }));
        }, []);

        const getOrderActionsFooter = useCallback(
            () => {
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
                            {t('SaveButton')}
                        </Button>
                    </>
                );
            },
            [orderForm, notChangeOrderForm],
        );

        const handleSave = () => {
            invokeAction('insert');
        };

        const handleDelete = () => {
            invokeAction('delete');
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

        const closeConfirmation = () => {
            setConfirmation({
                open: false,
            });
        };


        const invokeAction = actionName => {

            if (actionName == 'insert') {

                form.orders = !!form.orders ? form.orders : [];
                let orders = form.orders;

                if (indexRow == null)
                    orders.push(orderForm);
                else
                    orders[indexRow] = orderForm;

                onChangeForm(null, {name: 'orders', value: orders});
            }

            if (actionName == 'delete') {

                let orders = form.orders;
                orders = orders.slice(indexRow, indexRow + 1);

                onChangeForm(null, {name: 'orders', value: orders});
            }
            onCloseModal();
        };

        return (
            <>
                <CardLayout
                    title={title}
                    actionsFooter={actionsFooter}
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
                            error={error}
                            t={t}
                            form={orderForm}
                            onChange={onChangeOrderForm}
                            uniquenessNumberCheck={handleUniquenessCheck}
                        />
                    </Modal.Description>
                    <Modal.Actions>{getOrderActionsFooter()}</Modal.Actions>
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
