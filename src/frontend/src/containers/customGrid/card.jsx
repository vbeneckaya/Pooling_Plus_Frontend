import React, { useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';

import { Button, Dimmer, Loader, Modal } from 'semantic-ui-react';
import {
    cardSelector,
    editCardRequest,
    getCardRequest,
    openGridCardRequest,
    progressSelector,
} from '../../ducks/gridCard';
import OrderModal from '../../components/Modals/orderModal';
import ShippingModal from '../../components/Modals/shippingModal';

const getModal = {
    orders: <OrderModal />,
    shippings: <ShippingModal />,
};

const SelfComponent = props => {
    return React.cloneElement(<Card />, props);
};

const Card = props => {
    const { name, id, stopUpdate, loadList, title, children } = props;
    let [modalOpen, setModalOpen] = useState(false);
    let [form, setForm] = useState({});
    const { t } = useTranslation();
    const dispatch = useDispatch();

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

    const onOpen = () => {
        id && loadCard();
        stopUpdate && stopUpdate();
        setModalOpen(true);
    };

    const onClose = () => {
        setModalOpen(false);
        loadList(false, true);
    };

    const onChangeForm = (e, { name, value }) => {
        setForm({
            ...form,
            [name]: value,
        });
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

    const loading = useSelector(state => progressSelector(state));

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
            <Modal.Header>{t(title)}</Modal.Header>
            <Modal.Content scrolling>
                <Dimmer active={loading} inverted>
                    <Loader size="huge">Loading</Loader>
                </Dimmer>
                <Modal.Description>
                    {React.cloneElement(getModal[name], {
                        ...props,
                        form,
                        onChangeForm: onChangeForm,
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
                            title="yyyy"
                        >
                            <Button>Открыть перевозку</Button>
                        </SelfComponent>
                    ) : null}
                </div>
                <div>
                    <Button color="grey" onClick={onClose}>
                        {t('CancelButton')}
                    </Button>
                    <Button color="blue" onClick={handleSave}>
                        {t('SaveButton')}
                    </Button>
                </div>
            </Modal.Actions>
        </Modal>
    );
};

export default Card;
