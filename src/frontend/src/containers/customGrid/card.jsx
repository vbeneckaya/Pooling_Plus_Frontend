import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';

import { Button, Confirm, Dimmer, Loader, Modal } from 'semantic-ui-react';
import {
    cardSelector,
    editCardRequest,
    getCardRequest,
    openGridCardRequest,
    progressSelector,
} from '../../ducks/gridCard';
import OrderModal from '../../components/Modals/orderModal';
import ShippingModal from '../../components/Modals/shippingModal';
import {
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
        setForm({
            ...form,
            [name]: value,
        });

        if (notChangeForm && value !== undefined && Object.keys(form).length && value !== card[name]) {
            console.log('&&&&', name, value);
            setNotChangeForm(false);
        }
    };

    const handleSave = () => {
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
                        console.log('^^', actionName.toLowerCase(), actionName.toLowerCase().includes('delete'));
                        if (actionName.toLowerCase().includes('delete')) {
                            onClose()
                        } else {
                            loadCard();
                            getActions();
                        }
                    },
                }),
            );
        });
    };

    const loading = useSelector(state => progressSelector(state));
    const actions = useSelector(state => actionsSelector(state));
    const progressActionName = useSelector(state => progressActionNameSelector(state));

    console.log('title', title);

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
                    <Button
                        color="blue"
                        disabled={progressActionName || notChangeForm}
                        onClick={handleSave}
                    >
                        {t('SaveButton')}
                    </Button>
                </div>
            </Modal.Actions>
            <Confirm
                dimmer="blurring"
                open={confirmation.open}
                onCancel={confirmation.onCancel || closeConfirmation}
                onConfirm={confirmation.onConfirm}
                content={confirmation.content}
            />
        </Modal>
    );
};

export default Card;
