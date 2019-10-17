import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { Button, Confirm, Form, Input, Modal, Search } from 'semantic-ui-react';
import Text from '../BaseComponents/Text';
import DragAndDropFields from './DragAndDropFields';
import { columnsGridSelector } from '../../ducks/gridList';
import {
    deleteRepresentationRequest,
    editRepresentationRequest,
    representationNameSelector,
    representationSelector,
    representationsSelector,
    saveRepresentationRequest,
    setRepresentationRequest,
} from '../../ducks/representations';

const FieldsConfig = ({ children, title, gridName, isNew, getRepresentations }) => {
    const currName = useSelector(state => representationNameSelector(state, gridName));
    const currRepresentation = useSelector(state => representationSelector(state, gridName)) || [];

    let [modalOpen, setModalOpen] = useState(false);
    let [selectedFields, setSelectedFields] = useState(isNew ? [] : currRepresentation);
    let [name, setName] = useState(isNew ? '' : currName);
    let [error, setError] = useState(false);
    let [search, setSearch] = useState('');
    let [confirmation, setConfirmation] = useState({ open: false });

    const { t } = useTranslation();
    const dispatch = useDispatch();

    console.log('children', children);

    const fieldsList = useSelector(state => columnsGridSelector(state, gridName)).filter(column => {
        return !selectedFields.map(item => item.name).includes(column.name);
    });

    const list = useSelector(state => representationsSelector(state));

    useEffect(
        () => {
            !isNew ? setName(currName) : setName('');
        },
        [currName],
    );
    useEffect(
        () => {
            !isNew ? setSelectedFields(currRepresentation) : setSelectedFields([]);
        },
        [currRepresentation],
    );

    const onOpen = () => {
        !isNew ? setName(currName) : setName('');
        !isNew ? setSelectedFields(currRepresentation) : setSelectedFields([]);
        setModalOpen(true);
    };

    const onClose = callBackFunc => {
        getRepresentations && getRepresentations(callBackFunc);
        setModalOpen(false);
        setError(null);
        setConfirmation({open: false});
        setSearch('');
    };

    const onChange = selected => {
        setSelectedFields(selected);
    };

    const isNotUniqueName = () => {
        console.log('list[name]', list[name]);
        return Boolean(list[name]);
    };

    const handleSave = () => {
        if (!name) {
            setError('required_field');
        } else if (isNotUniqueName()) {
            setError('representation_already_exists');
        } else {
            dispatch(
                saveRepresentationRequest({
                    key: gridName,
                    name,
                    value: selectedFields,
                    callbackSuccess: () => {
                        onClose();
                    },
                }),
            );
        }
    };

    const handleEdit = () => {
        if (!name) {
            setError('required_field');
        } else if (isNotUniqueName() && name !== currName) {
            setError('representation_already_exists');
        } else {
            dispatch(
                editRepresentationRequest({
                    key: gridName,
                    name,
                    oldName: currName,
                    value: selectedFields,
                    callbackSuccess: () => {
                        onClose(() => {
                            dispatch(
                                setRepresentationRequest({
                                    gridName,
                                    value: name,
                                }),
                            );
                        });
                    },
                }),
            );
        }
    };

    const handleDelete = () => {
        showConfirmation(t('confirm_delete_representation'), () => {
            dispatch(
                deleteRepresentationRequest({
                    key: gridName,
                    name: currName,
                    callbackSuccess: () => {
                        onClose(() => {
                            dispatch(
                                setRepresentationRequest({
                                    gridName,
                                    value: null,
                                }),
                            );
                        });
                    },
                }),
            );
        });
    };

    const closeConfirmation = () => {
        setConfirmation({ open: false });
    };

    const showConfirmation = (content, onConfirm) => {
        setConfirmation({
            open: true,
            content,
            onConfirm,
        });
    };

    return (
        <Modal
            dimmer="blurring"
            id="fieldModal"
            trigger={children}
            open={modalOpen}
            onOpen={onOpen}
            onClose={onClose}
            closeOnEscape
            closeOnDimmerClick={false}
            closeIcon
        >
            <Modal.Header>{title}</Modal.Header>
            <Modal.Content>
                <Modal.Description>
                    <Form style={{ marginBottom: '16px' }}>
                        <Text
                            name="name"
                            value={name}
                            error={error}
                            errorText={
                                error && t(error)
                            }
                            onChange={(e, { value }) => setName(value)}
                        />
                        <Input
                            icon="search"
                            iconPosition="left"
                            placeholder={t('search_field')}
                            value={search}
                            clearable
                            onChange={(e, { value }) => setSearch(value)}
                        />
                    </Form>
                    <DragAndDropFields
                        type={gridName}
                        fieldsConfig={selectedFields}
                        fieldsList={fieldsList}
                        search={search}
                        onChange={onChange}
                    />
                </Modal.Description>
            </Modal.Content>
            <Modal.Actions className="grid-card-actions">
                <div>
                    <Button color="red" onClick={handleDelete}>
                        {t('delete')}
                    </Button>
                </div>
                <div>
                    <Button color="grey" onClick={onClose}>
                        {t('CancelButton')}
                    </Button>
                    <Button color="blue" onClick={isNew ? handleSave : handleEdit}>
                        {t('SaveButton')}
                    </Button>
                </div>
            </Modal.Actions>
            <Confirm
                dimmer="blurring"
                open={confirmation.open}
                onCancel={closeConfirmation}
                onConfirm={confirmation.onConfirm}
                content={confirmation.content}
            />
        </Modal>
    );
};

export default FieldsConfig;
