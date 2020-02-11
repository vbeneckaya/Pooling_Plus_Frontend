import React, {useCallback, useEffect, useMemo, useState} from 'react';
import {useDispatch, useSelector} from 'react-redux';
import {useTranslation} from 'react-i18next';

import {Button, Confirm, Dropdown, Icon, Popup} from 'semantic-ui-react';
import {
    cardSelector,
    clearGridCard,
    editCardRequest,
    editProgressSelector,
    errorSelector,
    getCardRequest,
    isUniqueNumberRequest,
    progressSelector,
    settingsFormSelector,
    settingsFormExtSelector,
} from '../../ducks/gridCard';
import {
    actionsCardSelector,
    clearActions,
    getActionsRequest,
    invokeActionRequest,
    progressActionNameSelector,
} from '../../ducks/gridActions';
import {ORDERS_GRID, SHIPPINGS_GRID} from '../../constants/grids';
import OrderCard from './components/orderCard';
import ShippingCard from './components/shippingCard';
import {GRID_CARD_LINK} from '../../router/links';
import {clearHistory, getHistoryRequest} from '../../ducks/history';
import {columnsGridSelector} from "../../ducks/gridList";
import {BIG_TEXT_TYPE, NUMBER_TYPE, TEXT_TYPE} from "../../constants/columnTypes";

const Card = props => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const {match, history, location} = props;
    const {params = {}} = match;
    const {name, id} = params;

    let [form, setForm] = useState({});
    let [allowToSend, setAllowToSend] = useState(false);
    // let [confirmation, setConfirmation] = useState({open: false});


    const card = useSelector(state => cardSelector(state));
    const settings = useSelector(state => settingsFormSelector(state, card.status));
    const error = useSelector(state => errorSelector(state));
    const columns = useSelector(state => columnsGridSelector(state, name));

    const title = useMemo(
        () =>
            id
                ? t(`edit_${name}`, {
                    number: name === ORDERS_GRID ? (!card.orderNumber ? card.orderNumber : card.orderNumber.value) :
                        (!!card.shippingNumber ? card.shippingNumber.value : card.shippingNumber),
                    status: t(form.status),
                })
                : t(`new_${name}`),
        [name, id, form],
    );

    useEffect(() => {
        dispatch(clearActions());
        id && loadCard();

        return () => {
            dispatch(clearHistory());
        };
    }, []);

    useEffect(
        () => {
            if (allowToSend) {
                handleSave();}
        },
        [form],
    );

    const loadCard = () => {
        id &&
        dispatch(
            getCardRequest({
                name,
                id,
                callbackSuccess: card => {
                    setForm(card);
                },
            }),
        );
        id &&
        dispatch(
            getActionsRequest({
                name,
                ids: [id],
                isCard: true,
            }),
        );
        id && dispatch(getHistoryRequest(id));
    };

    const onClose = () => {
        const {state} = location;
        if (!!state) {
            const {pathname, gridLocation} = state;
            history.replace({
                pathname: pathname,
                state: {
                    ...state,
                    pathname: gridLocation,
                },
            });
        }
    };

    const handleClose = () => {
        dispatch(clearGridCard());
        onClose();
    };

    const onChangeForm = useCallback((e, {name, value}) => {
        switch (name) {
            case 'clientId':
                setForm(prevState => ({
                    ...prevState,
                    [name]: value,
                    ['deliveryWarehouseId']: null
                }));
                break;
            case 'orderNumber':
            case 'shippingNumber':
                setForm(prevState => ({
                    ...prevState,
                    [name]: {value: value, name: null},
                }));
                break;
            default:
                setForm(prevState => ({
                    ...prevState,
                    [name]: value,
                }));
        }

        let column = columns.filter(_ => _.name == name)[0];
        let columnType = name == 'orderNumber' || name == 'shippingNumber' ? TEXT_TYPE : column && column.type;

        switch (columnType) {
            case TEXT_TYPE:
            case BIG_TEXT_TYPE:
            case NUMBER_TYPE:
                setAllowToSend(false);
                break;
            default:
                setAllowToSend(true);
        }
    }, []);

    const onBlurForm = () => {
        handleSave();
    };


    const saveOrEditForm = () => {
        dispatch(
            editCardRequest({
                name,
                params: form,
                callbackSuccess: (result) => {
                    setAllowToSend(false);
                    if (form.id) {
                        loadCard();
                    }
                    else {
                        goToCardFromNewCard(name, result.id);
                    }
                 }
            }),
        );
    };

    const handleSave = () => {
        if (name === ORDERS_GRID) {
            handleUniquenessCheck(saveOrEditForm);
        } else {
            saveOrEditForm();
        }
    };

    // const closeConfirmation = () => {
    //     setConfirmation({
    //         open: false,
    //     });
    // };

    // const showConfirmation = (content, onConfirm, onCancel) => {
    //     setConfirmation({
    //         open: true,
    //         content,
    //         onConfirm,
    //         onCancel,
    //     });
    // };

    const invokeAction = actionName => {
        // showConfirmation(
        //     `${t('Are you sure to complete')} "${t(actionName)}"?`,
        //     () => {
        //         closeConfirmation();
        dispatch(
            editCardRequest({
                name,
                params: form,
                callbackSuccess: () => {
                    dispatch(
                        invokeActionRequest({
                            ids: [id],
                            name,
                            actionName,
                            callbackSuccess: () => {
                                if (actionName.toLowerCase().includes('delete')) {
                                    onClose();
                                } else {
                                    loadCard();
                                }
                            },
                        }),
                    );
                },
            }),
        );
        // },
        //     closeConfirmation,
        // );
    };

    const handleUniquenessCheck = callbackFunc => {
        if (form.orderNumber && (!id || form.orderNumber.value !== card.orderNumber.value)) {
            dispatch(
                isUniqueNumberRequest({
                    number: !!form.orderNumber ? form.orderNumber.value : null,
                    fieldName: 'orderNumber',
                    errorText: t('number_already_exists'),
                    callbackSuccess: callbackFunc,
                }),
            );
        }
        else callbackFunc();
    };

    const loading = useSelector(state => progressSelector(state));
    const editLoading = useSelector(state => editProgressSelector(state));
    const actions = useSelector(state => actionsCardSelector(state));
    const progressActionName = useSelector(state => progressActionNameSelector(state));
    //  const disableSave = progressActionName || notChangeForm;

    // const getActionsFooter = useCallback(
    //     () => {
    //         return (
    //             <>
    //                 <Button color="grey" onClick={handleClose}>
    //                     {t('CancelButton')}
    //                 </Button>
    //                 <Button
    //                     color="blue"
    //                     disabled={disableSave}
    //                     loading={editLoading}
    //                     onClick={handleSave}
    //                 >
    //                     {id ? t('SaveButton') : t('create_btn')}
    //                 </Button>
    //             </>
    //         );
    //     },
    //     [form, disableSave, editLoading, name],
    // );

    const goToCard = (gridName, cardId) => {
        const {state} = location;
        history.replace({
            pathname: GRID_CARD_LINK.replace(':name', gridName).replace(':id', cardId),
            state: {
                ...state,
                pathname: history.location.pathname,
                gridLocation: state.gridLocation ? state.gridLocation : state.pathname,
            },
        });
    };

    const goToCardFromNewCard = (gridName, cardId) => {
        history.replace({
            pathname: GRID_CARD_LINK.replace(':name', gridName).replace(':id', cardId),
        });
    };

    const getActionsHeader = useCallback(
        () => {
            return (
                <div className="grid-card-header">
                    {name === ORDERS_GRID && form.shippingId ? (
                        <div
                            className="link-cell"
                            onClick={() => goToCard(SHIPPINGS_GRID, form.shippingId)}
                        >
                            {t('open_shipping', {number: form.shippingNumber.value})}
                        </div>
                    ) : null}
                    {/*{name === SHIPPINGS_GRID && form.orders && form.orders.length ? (*/}
                    {/*<Dropdown*/}
                    {/*text={t('orders')}*/}
                    {/*pointing="top right"*/}
                    {/*className="dropdown-blue"*/}
                    {/*scrolling*/}
                    {/*>*/}
                    {/*<Dropdown.Menu>*/}
                    {/*{form.orders.map(order => (*/}
                    {/*<Dropdown.Item*/}
                    {/*className="link-cell"*/}
                    {/*key={order.id}*/}
                    {/*text={order.orderNumber}*/}
                    {/*onClick={() => {*/}
                    {/*goToCard(ORDERS_GRID, order.id);*/}
                    {/*}}*/}
                    {/*/>*/}
                    {/*))}*/}
                    {/*</Dropdown.Menu>*/}
                    {/*</Dropdown>*/}
                    {/*) : null}*/}

                    {/* <Dropdown
                        icon="ellipsis horizontal"
                        floating
                        button
                        pointing="top right"
                        className="icon"
                        scrolling
                    >
                        <Dropdown.Menu>
                            {actions &&
                            actions.filter(item => item.allowedFromForm).map(action => (
                                <Dropdown.Item
                                    key={action.name}
                                    text={t(action.name)}
                                    label={{
                                        color: action.color,
                                        empty: true,
                                        circular: true,
                                    }}
                                    onClick={() => invokeAction(action.name)}
                                />
                            ))}
                        </Dropdown.Menu>
                    </Dropdown>*/}
                    {actions &&
                    actions.filter(item => item.allowedFromForm).map(action => (
                        <Popup
                            content={action.description}
                            disabled={!action.description}
                            trigger={
                                <Button
                                    className="grid-card-header_actions_button"
                                    key={action.name}
                                    loading={action.loading}
                                    disabled={action.loading}
                                    size="mini"
                                    compact
                                    onClick={() => invokeAction(action.name)}
                                >
                                    <Icon name="circle" color={action.color}/>
                                    {t(action.name)}
                                </Button>
                            }
                        />
                    ))}
                </div>
            );
        },
        [form, actions, name],
    );

    return (
        <React.Fragment>
            {name === ORDERS_GRID ? (
                <OrderCard
                    {...props}
                    id={id}
                    load={loadCard}
                    name={name}
                    form={form}
                    title={title}
                    settings={settings}
                    loading={loading}
                    uniquenessNumberCheck={handleSave} //{handleUniquenessCheck}
                    error={error}
                    onClose={handleClose}
                    onChangeForm={onChangeForm}
                    onBlurForm={onBlurForm}
                    //  actionsFooter={getActionsFooter}
                    actionsHeader={getActionsHeader}
                />
            ) : (
                <ShippingCard
                    {...props}
                    title={title}
                    id={id}
                    name={name}
                    form={form}
                    load={loadCard}
                    loading={loading}
                    settings={settings}
                    error={error}
                    onClose={handleClose}
                    onBlurForm={onBlurForm}
                    onChangeForm={onChangeForm}
                    //  actionsFooter={getActionsFooter}
                    actionsHeader={getActionsHeader}
                />
            )}
            {/*<Confirm*/}
            {/*dimmer="blurring"*/}
            {/*open={confirmation.open}*/}
            {/*onCancel={confirmation.onCancel || closeConfirmation}*/}
            {/*cancelButton={t('cancelConfirm')}*/}
            {/*confirmButton={t('Yes')}*/}
            {/*onConfirm={confirmation.onConfirm}*/}
            {/*content={confirmation.content}*/}
            {/*/>*/}
        </React.Fragment>
    );
};

export default Card;
