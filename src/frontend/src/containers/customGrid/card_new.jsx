import React, {useState, useEffect, useCallback, useMemo} from 'react';
import {useDispatch, useSelector} from 'react-redux';
import {useTranslation} from 'react-i18next';

import {Button, Confirm, Dimmer, Loader, Modal} from 'semantic-ui-react';
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
import OrderCard from "./components/orderCard";
import ShippingCard from "./components/shippingCard";

const getModal = {
    orders: <OrderModal/>,
    shippings: <ShippingModal/>,
};

const SelfComponent = props => {
    return React.cloneElement(<Card/>, props);
};

const Card = props => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const {match} = props;
    const {params = {}} = match;
    const {name, id} = params;

    let [form, setForm] = useState({});
    let [notChangeForm, setNotChangeForm] = useState(true);
    let [confirmation, setConfirmation] = useState({open: false});
    let [isNotUniqueNumber, setIsNotUnqueNumber] = useState(false);

    const title = useMemo(
        () => (id ? t(`edit_${name}`) : t(`new_${name}`)),
        [name, id],
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

            setNotChangeForm(true);
            dispatch(clearGridCard());
            setForm({});
            setIsNotUnqueNumber(false);
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
        setConfirmation({open: false});
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

    return (
        <>
            {name === ORDERS_GRID ? (
                <OrderCard
                    {...props}
                    form={form}
                    load={loadCard}
                    settings={settings}
                    uniquenessNumberCheck={handleUniquenessCheck}
                    error={error}
                    onClose={onClose}
                    onChangeForm={onChangeForm}
                />
            ) : (
                <ShippingCard
                    {...props}
                    form={form}
                    load={loadCard}
                    settings={settings}
                    error={error}
                    onClose={onClose}
                    onChangeForm={onChangeForm}
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
