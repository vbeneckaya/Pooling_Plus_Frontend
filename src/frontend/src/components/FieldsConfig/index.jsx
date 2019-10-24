import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { Button, Confirm, Form, Input, Label, Message, Modal, Search, Dropdown, Grid } from 'semantic-ui-react';
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

const FieldsConfig = ({ title, gridName, getRepresentations, representation, changeRepresentation, representations }) => {
    const currName = useSelector(state => representationNameSelector(state, gridName));
    const currRepresentation = useSelector(state => representationSelector(state, gridName)) || [];
    let [isNew, setIsNew] = useState(true);
    let [modalOpen, setModalOpen] = useState(false);
    let [selectedFields, setSelectedFields] = useState(isNew ? [] : currRepresentation);
    let [name, setName] = useState(isNew ? '' : currName);
    let [error, setError] = useState(false);
    let [isEmpty, setEmpty] = useState(false);
    let [search, setSearch] = useState('');
    let [confirmation, setConfirmation] = useState({ open: false });

    const { t } = useTranslation();
    const dispatch = useDispatch();

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

    const onOpen = (isNew) => {
        setIsNew(isNew);
        !isNew ? setName(currName) : setName('');
        !isNew ? setSelectedFields(currRepresentation) : setSelectedFields([]);
        setModalOpen(true);
    };

    const onClose = callBackFunc => {
        getRepresentations && getRepresentations(callBackFunc);
        setModalOpen(false);
        setError(null);
        setEmpty(false);
        setConfirmation({ open: false });
        setSearch('');
    };

    const onChange = selected => {
        setSelectedFields(selected);
        setEmpty(false);
    };

    const isNotUniqueName = () => {
        return Boolean(list[name]);
    };

    const handleSave = () => {
        if (!name) {
            setError('required_field');
        } else if (isNotUniqueName()) {
            setError('representation_already_exists');
        } else if (!selectedFields.length) {
            setError(null);
            setEmpty(true);
        } else {
            dispatch(
                saveRepresentationRequest({
                    key: gridName,
                    name,
                    value: selectedFields,
                    callbackSuccess: () => {

                        onClose(() => {
                            changeRepresentation(name);
                           /* dispatch(
                                setRepresentationRequest({
                                    gridName,
                                    value: name,
                                }),
                            );*/
                        });
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
        } else if (!selectedFields.length) {
            setError(null);
            setEmpty(true);
        } else {
            dispatch(
                editRepresentationRequest({
                    key: gridName,
                    name,
                    oldName: currName,
                    value: selectedFields,
                    callbackSuccess: () => {
                        onClose(() => {
                            changeRepresentation(name);
                           /* dispatch(
                                setRepresentationRequest({
                                    gridName,
                                    value: name,
                                }),
                            );*/
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
                            changeRepresentation(null);
                            /*dispatch(
                                setRepresentationRequest({
                                    gridName,
                                    value: null,
                                }),
                            );*/
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
        <>
            <div className="representation">
                <label>{t('representation')}</label>
                <Form.Field>
                    <Dropdown
                        selection
                        text={representation || t('default_representation')}
                        fluid
                    >
                        <Dropdown.Menu>
                            <Dropdown.Item
                                text={t('default_representation')}
                                onClick={() => changeRepresentation(null)}
                            />
                            {representations && Object.keys(representations).length ? (
                                <>
                                    {Object.keys(representations)
                                        .sort()
                                        .map(key => (
                                            <Dropdown.Item
                                                text={key}
                                                onClick={() => changeRepresentation(key)}
                                            />
                                        ))}
                                </>
                            ) : null}
                            <Dropdown.Divider />
                            <Dropdown.Item icon="add" text={t('create_btn')} onClick={() => onOpen(true)}/>
                        </Dropdown.Menu>
                    </Dropdown>
                </Form.Field>
                    <Button icon="cogs" disabled={!representation} onClick={() => onOpen(false)}/>
            </div>
            <Modal
                dimmer="blurring"
                id="fieldModal"
                open={modalOpen}
                onClose={onClose}
                className="representation-modal"
                closeIcon
            >
                <Modal.Header>{isNew ? t('Create representation') : t('Edit representation', { name: representation })}</Modal.Header>
                <Modal.Content>
                    <Modal.Description>
                        <Form style={{ marginBottom: '16px' }}>
                            <Text
                                name="name"
                                value={name}
                                error={error}
                                errorText={error && t(error)}
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
                        {isEmpty ? (
                            <Message negative>{t('Добавьте поля в представление')}</Message>
                        ) : null}
                    </Modal.Description>
                </Modal.Content>
                <Modal.Actions className="grid-card-actions">
                    <div>
                        {!isNew ? (
                            <Button color="red" onClick={handleDelete}>
                                {t('delete')}
                            </Button>
                        ) : null}
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
                    cancelButton={t('cancelConfirm')}
                    onConfirm={confirmation.onConfirm}
                    content={confirmation.content}
                />
            </Modal>
        </>
    );
};

export default FieldsConfig;
