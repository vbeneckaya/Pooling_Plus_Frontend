import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';

import { Button, Confirm, Dimmer, Loader, Modal } from 'semantic-ui-react';
import {
    cardSelector,
    editCardRequest,
    getCardRequest,
    isUniqueNumberRequest,
    openGridCardRequest,
    progressSelector,
} from '../../ducks/gridCard';
import OrderModal from '../../components/Modals/orderModal';
import ShippingModal from '../../components/Modals/shippingModal';
import {
    actionsCardSelector,
    actionsSelector,
    clearActions,
    getActionsRequest,
    invokeActionRequest,
    progressActionNameSelector,
} from '../../ducks/gridActions';

const getModal = {
    orders: <OrderModal />,
    shippings: <ShippingModal />,
};

const SelfComponent = props => {
    return React.cloneElement(<Card />, props);
};

const Card = props => {
    const { name, id, stopUpdate, loadList, title, children, onClose: beforeClose } = props;
    let [modalOpen, setModalOpen] = useState(false);
    let [form, setForm] = useState({});
    let [notChangeForm, setNotChangeForm] = useState(true);
    let [confirmation, setConfirmation] = useState({ open: false });
    let [isNotUniqueNumber, setIsNotUnqueNumber] = useState(false);
    const { t } = useTranslation();
    const dispatch = useDispatch();

    const card = useSelector(state => cardSelector(state));

    const loadCard = () => {
        dispatch(
            getCardRequest({
                name,
                id,
                callbackSuccess: card => {
                    setForm(card);
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

    const onOpen = () => {
        dispatch(clearActions());
        id && loadCard();
        id && getActions();
        stopUpdate && stopUpdate();
        setModalOpen(true);
    };

    const onClose = isConfirm => {
        if (!isConfirm || notChangeForm) {
            beforeClose ? beforeClose() : setModalOpen(false);
            setForm({});
            setIsNotUnqueNumber(false);
            loadList && loadList(false, true);
        } else {
            showConfirmation(
                t('confirm_close'),
                () => {
                    closeConfirmation();
                    handleSave();
                },
                () => {
                    closeConfirmation();
                    onClose();
                },
            );
        }
    };

    const onChangeForm = (e, { name, value }) => {
        console.log('va', value, name, card[name], value !== card[name]);
        setForm({
            ...form,
            [name]: value,
        });

        if (
            notChangeForm &&
            value !== undefined &&
            Object.keys(form).length &&
            value !== card[name]
        ) {
            setNotChangeForm(false);
        }
    };

    const saveOrEditForm = () => {
        dispatch(
            editCardRequest({
                name,
                params: form,
                callbackSuccess: () => {
                    onClose();
                },
            }),
        );
    };

    const handleSave = () => {
        if (name === 'orders') {
            handleUniquenessCheck(saveOrEditForm);
        } else {
            saveOrEditForm();
        }
    };

    const closeConfirmation = () => {
        setConfirmation({ open: false });
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
        showConfirmation(`${t('Are you sure to complete')} "${t(actionName)}"?`, () => {
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
        });
    };

    const handleUniquenessCheck = callbackFunc => {
        dispatch(
            isUniqueNumberRequest({
                number: form.orderNumber,
                callbackSuccess: number => {
                    if (number && number !== card.orderNumber) {
                        setIsNotUnqueNumber(true);
                    } else {
                        setIsNotUnqueNumber(false);
                        callbackFunc && callbackFunc();
                    }
                },
            }),
        );
    };

    const loading = useSelector(state => progressSelector(state));
    const actions = useSelector(state => actionsCardSelector(state));
    const progressActionName = useSelector(state => progressActionNameSelector(state));
    const disableSave = progressActionName || notChangeForm || isNotUniqueNumber;

    return (
        <Modal
            dimmer="blurring"
            className="card-modal"
            trigger={children}
            open={modalOpen}
            onOpen={onOpen}
            onClose={onClose}
            closeIcon
            size="large"
        >
            <Modal.Header>
                {t(title, {
                    number: name === 'orders' ? form.orderNumber : form.shippingNumber,
                    status: t(form.status),
                })}
            </Modal.Header>
            <Modal.Content scrolling>
                <Dimmer active={loading} inverted>
                    <Loader size="huge">Loading</Loader>
                </Dimmer>
                <Modal.Description>
                    {React.cloneElement(getModal[name], {
                        ...props,
                        form,
                        load: loadCard,
                        uniquenessNumberCheck: handleUniquenessCheck,
                        isNotUniqueNumber,
                        onClose,
                        onChangeForm,
                    })}
                </Modal.Description>
            </Modal.Content>
            <Modal.Actions className="grid-card-actions">
                <div>
                    {name === 'orders' && form.shippingId ? (
                        <SelfComponent
                            {...props}
                            name="shippings"
                            id={form.shippingId}
                            title={`edit_shippings`}
                            onClose={onClose}
                        >
                            <Button>{t('open_shipping', { number: form.shippingNumber })}</Button>
                        </SelfComponent>
                    ) : null}
                    {actions &&
                        actions.map(item => (
                            <Button
                                color={item.color}
                                loading={progressActionName === item.name}
                                disabled={progressActionName}
                                onClick={() => invokeAction(item.name)}
                            >
                                {t(item.name)}
                            </Button>
                        ))}
                </div>
                <div>
                    <Button
                        color="grey"
                        disabled={progressActionName}
                        onClick={() => onClose(true)}
                    >
                        {t('CancelButton')}
                    </Button>
                    <Button color="blue" disabled={disableSave} onClick={handleSave}>
                        {t('SaveButton')}
                    </Button>
                </div>
            </Modal.Actions>
            <Confirm
                dimmer="blurring"
                open={confirmation.open}
                onCancel={confirmation.onCancel || closeConfirmation}
                cancelButton={t('cancelConfirm')}
                onConfirm={confirmation.onConfirm}
                content={confirmation.content}
            />
        </Modal>
    );
};

export default Card;
