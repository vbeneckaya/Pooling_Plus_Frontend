import React, {useCallback, useEffect, useMemo, useState} from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useTranslation } from 'react-i18next';

import {Button, Confirm, Dropdown} from 'semantic-ui-react';
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

const Card = props => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const {match, history, location} = props;
    const {params = {}} = match;
    const {name, id} = params;

    let [form, setForm] = useState({});
    let [notChangeForm, setNotChangeForm] = useState(true);
    let [confirmation, setConfirmation] = useState({ open: false });

    const title = useMemo(
        () =>
            id
                ? t(`edit_${name}`, {
                    number: name === ORDERS_GRID ? form.orderNumber : form.shippingNumber,
                    status: t(form.status),
                })
                : t(`new_${name}`),
        [name, id, form],
    );

    const card = useSelector(state => cardSelector(state));
    const settings = useSelector(state => settingsFormSelector(state, card.status));
    const error = useSelector(state => errorSelector(state));

    useEffect(() => {
        dispatch(clearActions());
        id && loadCard();

        return () => {
            dispatch(clearHistory());
        };
    }, []);

    useEffect(
        () => {
            if (notChangeForm) {
                Object.keys(form).forEach(key => {
                    if (form[key] !== card[key]) {
                        setNotChangeForm(false);
                    }
                });
            }
        },
        [form],
    );

    const loadCard = () => {
        id && dispatch(
            getCardRequest({
                name,
                id,
                callbackSuccess: card => {
                    setForm(card);
                    setNotChangeForm(true);
                },
            }),
        );
        id && dispatch(
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
        const {pathname, gridLocation} = state;

        history.replace({
            pathname: pathname,
            state: {
                ...state,
                pathname: gridLocation,
            },
        });
    };

    const handleClose = isConfirm => {
        if (!isConfirm || notChangeForm) {
            dispatch(clearGridCard());
            onClose();
        } else {
            showConfirmation(
                t('confirm_close_dictionary'),
                () => {
                    closeConfirmation();
                    onClose();
                },
                () => {
                    closeConfirmation();
                },
            );
        }
    };

    const onChangeForm = useCallback((e, {name, value}) => {
        setForm(prevState => ({
            ...prevState,
            [name]: value,
        }));
    }, []);

    const saveOrEditForm = () => {
        dispatch(
            editCardRequest({
                name,
                params: form,
                callbackSuccess: () => {
                    if (form.id) {
                        loadCard();
                    } else {
                        handleClose();
                    }
                },
            }),
        );
    };

    const handleSave = () => {
        saveOrEditForm();
        // if (name === ORDERS_GRID && !form.id) {
        //     handleUniquenessCheck(saveOrEditForm);
        // } else {
        //     saveOrEditForm();
        // }
    };

    const closeConfirmation = () => {
        setConfirmation({
            open: false,
        });
    };

    const showConfirmation = (content, onConfirm, onCancel) => {
        setConfirmation({
            open: true,
            content,
            onConfirm,
            onCancel,
        });
    };

    const invokeAction = actionName => {
        showConfirmation(
            `${t('Are you sure to complete')} "${t(actionName)}"?`,
            () => {
                closeConfirmation();
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
            closeConfirmation,
        );
    };

    const handleUniquenessCheck = callbackFunc => {
        dispatch(
            isUniqueNumberRequest({
                number: form.orderNumber,
                fieldName: 'orderNumber',
                errorText: t('number_already_exists'),
                callbackSuccess: callbackFunc,
            }),
        );
    };

    const loading = useSelector(state => progressSelector(state));
    const editLoading = useSelector(state => editProgressSelector(state));
    const actions = useSelector(state => actionsCardSelector(state));
    const progressActionName = useSelector(state => progressActionNameSelector(state));
    const disableSave = progressActionName || notChangeForm;

    const getActionsFooter = useCallback(
        () => {
            return (
                <>
                    <Button color="grey" onClick={handleClose}>
                        {t('CancelButton')}
                    </Button>
                    <Button
                        color="blue"
                        disabled={disableSave}
                        loading={editLoading}
                        onClick={handleSave}
                    >
                        {t('SaveButton')}
                    </Button>
                </>
            );
        },
        [form, disableSave, editLoading, name],
    );

    const goToCard = (gridName, cardId) => {
        const {state} = location;
        console.log('to', state);
        history.replace({
            pathname: GRID_CARD_LINK.replace(':name', gridName).replace(':id', cardId),
            state: {
                ...state,
                pathname: history.location.pathname,
                gridLocation: state.gridLocation ? state.gridLocation : state.pathname,
            },
        });
    };

    const getActionsHeader = useCallback(
        () => {
            return (
                <div
                    className="grid-card-header"
                    onClick={() => goToCard(SHIPPINGS_GRID, form.shippingId)}
                >
                    {name === ORDERS_GRID && form.shippingId ? (
                        <div className="link-cell">
                            {t('open_shipping', {number: form.shippingNumber})}
                        </div>
                    ) : null}
                    {name === SHIPPINGS_GRID && form.orders && form.orders.length ? (
                        <Dropdown
                            text={t('orders')}
                            pointing="top right"
                            className="dropdown-blue"
                            scrolling
                        >
                            <Dropdown.Menu>
                                {form.orders.map(order => (
                                    <Dropdown.Item
                                        className="link-cell"
                                        key={order.id}
                                        text={order.orderNumber}
                                        onClick={() => {
                                            goToCard(ORDERS_GRID, order.id);
                                        }}
                                    />
                                ))}
                            </Dropdown.Menu>
                        </Dropdown>
                    ) : null}
                    <Dropdown
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
                    </Dropdown>
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
                    uniquenessNumberCheck={handleUniquenessCheck}
                    error={error}
                    onClose={handleClose}
                    onChangeForm={onChangeForm}
                    actionsFooter={getActionsFooter}
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
                    onChangeForm={onChangeForm}
                    actionsFooter={getActionsFooter}
                    actionsHeader={getActionsHeader}
                />
            )}
            <Confirm
                dimmer="blurring"
                open={confirmation.open}
                onCancel={confirmation.onCancel || closeConfirmation}
                cancelButton={t('cancelConfirm')}
                confirmButton={t('Yes')}
                onConfirm={confirmation.onConfirm}
                content={confirmation.content}
            />
        </React.Fragment>
    );
};

export default Card;
