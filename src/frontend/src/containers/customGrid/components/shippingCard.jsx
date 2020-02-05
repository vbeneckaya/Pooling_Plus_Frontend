import React, {useCallback, useEffect, useMemo, useState} from 'react';
import {useTranslation} from 'react-i18next';
import Information from './shippingTabs/information';
import Routes from './shippingTabs/routes';
import Documents from './shared/documents';
import History from './shared/history';
import Accounts from './shippingTabs/accounts';
import {useDispatch, useSelector} from 'react-redux';
import {userPermissionsSelector} from '../../../ducks/profile';
import CardLayout from '../../../components/CardLayout';
import Orders from "./shippingTabs/orders";
import {Button, Confirm, Form, Grid, Modal, Segment} from "semantic-ui-react";
import FormField from "../../../components/BaseComponents";
import {ordersMiniColumns} from "../../../constants/ordersMiniColumns";
import {DATE_TYPE, NUMBER_TYPE, SELECT_TYPE, TEXT_TYPE} from "../../../constants/columnTypes";
import {editCardRequest, isUniqueNumberRequest} from "../../../ducks/gridCard";

const Content = ({t, error, form, onChange, uniquenessNumberCheck, isNotUniqueNumber}) => {
    const extSearchParamsFromDeliveryWarehouse = useMemo(() => ({
        clientId: form['clientId'] ? form['clientId'].value : undefined,
    }), [form['clientId']]);
    
    console.log('form from modal', form);
    console.log('error from modal', error);

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
                            error={(isNotUniqueNumber && t('number_already_exists')) || error['orderNumber']}
                            onBlur={uniquenessNumberCheck}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="clientOrderNumber"
                            type={TEXT_TYPE}
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
                // if (notChangeOrderForm) {
                //     Object.keys(orderForm).forEach(key => {
                //         if (orderForm[key] !== orderCard[key]) {
                //             setNotChangeOrderForm(false);
                //         }
                //     });
                // }
               // setNotChangeOrderForm(true);
            },
            [orderForm],
        );

        const handleCreateOrder = () => {
            setIndexRow(null);
            orderForm['id'] = null;
            orderColumns.forEach(column => {
                orderForm[column.name] = null
            });
            setOrderCard(orderForm);
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
                        onChange={onChangeForm}
                        openOrderModal={openOrderModal}
                        settings={settings}/>
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
        },[]);

        const getActionsFooter = useCallback(
            () => {
                return (
                    <>
                        <Button color="grey" onClick={handleClose}>
                            {t('CancelButton')}
                        </Button>
                        <Button
                            color="blue"
                           // disabled={notChangeOrderForm}
                            onClick={handleSave}
                        >
                            {t('SaveButton')}
                        </Button>
                    </>
                );
            },
            [form, notChangeOrderForm],
        );

        const handleSave = () => {

            invokeAction('insert');
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
                let orders = form.orders;
                if (indexRow == null) {
                    orders.push(orderForm);
                } else {
                    orders[indexRow] = orderForm;
                }
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
                        <Modal.Actions>{getActionsFooter()}</Modal.Actions>
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
