import React, {useState, useEffect, useCallback, useMemo} from 'react';
import {useDispatch, useSelector} from 'react-redux';
import {useTranslation} from 'react-i18next';
import {withRouter} from 'react-router-dom';

import {Button, Confirm, Dimmer, Dropdown, Icon, Loader, Modal, Popup} from 'semantic-ui-react';
import {
    cardSelector,
    clearGridCard,
    editCardRequest,
    errorSelector,
    getCardRequest,
    isUniqueNumberRequest,
    progressSelector,
    settingsFormSelector,
} from '../../ducks/gridCard';
import OrderModal from '../../components/Modals/orderModal';
import ShippingModal from '../../components/Modals/shippingModal';
import {
    actionsCardSelector,
    clearActions,
    getActionsRequest,
    invokeActionRequest,
    progressActionNameSelector,
} from '../../ducks/gridActions';
import {ORDERS_GRID} from '../../constants/grids';
import OrderCard from './components/orderCard';
import ShippingCard from './components/shippingCard';

const Card = props => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const {match, history, location} = props;
    const {params = {}} = match;
    const {name, id} = params;

    let [form, setForm] = useState({});
    let [notChangeForm, setNotChangeForm] = useState(true);
    let [confirmation, setConfirmation] = useState({open: false});

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
        id && getActions();
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
        dispatch(
            getCardRequest({
                name,
                id,
                callbackSuccess: card => {
                    setForm(card);
                    setNotChangeForm(true);
                },
            }),
        );
    };

    const getActions = () => {
        dispatch(
            getActionsRequest({
                name,
                ids: [id],
                isCard: true,
            }),
        );
    };

    const onClose = isConfirm => {
        if (!isConfirm || notChangeForm) {
            dispatch(clearGridCard());
            closeConfirmation();
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
                        getActions();
                    } else {
                        onClose();
                    }
                },
            }),
        );
    };

    const handleSave = () => {
        if (name === ORDERS_GRID && !form.id) {
            handleUniquenessCheck(saveOrEditForm);
        } else {
            saveOrEditForm();
        }
    };

    const closeConfirmation = () => {
        history.push({
            pathname: location.state.pathname,
            state: {...location.state},
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
                                getActions();
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
    const actions = useSelector(state => actionsCardSelector(state));
    const progressActionName = useSelector(state => progressActionNameSelector(state));
    const disableSave = progressActionName || notChangeForm;
    const progress = false;

    const getActionsFooter = useCallback(
        () => {
            return (
                <>
                    <Button color="grey" onClick={onClose}>
                        {t('CancelButton')}
                    </Button>
                    <Button
                        color="blue"
                        disabled={disableSave}
                        loading={progress}
                        onClick={handleSave}
                    >
                        {t('SaveButton')}
                    </Button>
                </>
            );
        },
        [form, disableSave, progress],
    );

    const getActionsHeader = useCallback(() => {
        return (
            <div className="grid-card-header">
                {name === ORDERS_GRID && form.shippingId ? (
                    <div className="link-cell">{t('open_shipping', {number: form.shippingNumber})}</div>
                ) : null}
                <Dropdown
                    icon="ellipsis horizontal"
                    floating
                    button
                    pointing="top right"
                    className="icon"
                >
                    <Dropdown.Menu>
                        <Dropdown.Menu scrolling>
                            {
                                actions && actions.map(action => (
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
                                    )
                                )
                            }
                        </Dropdown.Menu>
                    </Dropdown.Menu>
                </Dropdown>
            </div>
        );
    }, [form]);

    return (
        <>
            {name === ORDERS_GRID ? (
                <OrderCard
                    {...props}
                    id={id}
                    name={name}
                    form={form}
                    title={title}
                    settings={settings}
                    uniquenessNumberCheck={handleUniquenessCheck}
                    error={error}
                    onClose={onClose}
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
                    settings={settings}
                    error={error}
                    onClose={onClose}
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
        </>
    );
};

export default Card;
