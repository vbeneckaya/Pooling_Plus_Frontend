import React, {useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';
import {useDispatch, useSelector} from 'react-redux';
import {Button, Confirm, Dropdown, Form, Input, Message, Modal, Popup} from 'semantic-ui-react';
import Text from '../../BaseComponents/Text';
import {columnsGridSelector} from '../../../ducks/gridList';
import {
    deleteRepresentationRequest,
    editRepresentationRequest,
    getRepresentationsRequest,
    getDefaultRepresentationRequest,
    representationSelector,
    representationsSelector,
    saveRepresentationRequest,
} from '../../../ducks/representations';
import DragAndDrop from "../../DragAndDrop";

const FieldsConfig = ({
                          gridName,
                          getRepresentations,
                          changeRepresentation,
                          representations,
                          representationName,
                          isEditDefaultRepresentation
                      }) => {
    const representationFields =
        useSelector(state => representationSelector(state, gridName)) || [];

    let [modalOpen, setModalOpen] = useState(false);
    let [isNew, setIsNew] = useState(true);
    let [isDefault, setIsDefault] = useState(true);
    let [selectedFields, setSelectedFields] = useState([]);
    let [name, setName] = useState('');
    let [error, setError] = useState(false);
    let [isEmpty, setEmpty] = useState(false);
    let [search, setSearch] = useState('');
    let [confirmation, setConfirmation] = useState({open: false});

    const {t} = useTranslation();
    const dispatch = useDispatch();

    const fieldsList = useSelector(state => columnsGridSelector(state, gridName)).filter(column => {
        return !selectedFields.map(item => item.name).includes(column.name);
    });

    const list = useSelector(state => representationsSelector(state));

    useEffect(
        () => {
            setSelectedFields(representationFields);
        },
        [representationFields],
    );

    useEffect(
        () => {
            setSelectedFields(representationFields);
        },
        [representationFields],
    );

    const newOpen = () => {
        setIsNew(true);
        setIsDefault(false);
        setName('');
        setSelectedFields([]);
        onOpen();
    };

    const editOpen = () => {
        if (isDefault) {
            dispatch(
                getDefaultRepresentationRequest({
                    key: gridName,
                }),
            );
        }
        else {
            dispatch(
                getRepresentationsRequest({
                    key: gridName,
                }),
            );
        }
        setIsNew(false);
        setName(representationName);
        setSelectedFields(representationFields);
        onOpen();
    };

    const onOpen = () => {
        setModalOpen(true);
    };

    const onClose = (newName) => {
        let updateView = typeof(newName) == "string" || newName == null;
        getRepresentations && updateView && getRepresentations({representationToSetName: newName});
        setModalOpen(false);
        setError(null);
        setEmpty(false);
        setConfirmation({open: false});
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
        if (isDefault) {
            if (!selectedFields.length) {
                setError(null);
                setEmpty(true);
            } else
                dispatch(
                    saveRepresentationRequest({
                        key: gridName,
                        name,
                        isDefault: true,
                        value: selectedFields,
                        callbackSuccess: () => {
                        },
                    }),
                );
        }
        else {
            if (!name) {
                setError('required_field');
            } else if (isNotUniqueName()) {
                setError('representation_already_exists');
            } else if (!selectedFields.length) {
                setError(null);
                setEmpty(true);
            } else
                dispatch(
                    saveRepresentationRequest({
                        key: gridName,
                        name,
                        isDefault: false,
                        value: selectedFields,
                        callbackSuccess: () => {
                            onClose(name);
                        },
                    }),
                );
        }
    };

    const handleEdit = () => {
        if (isDefault) {
            if (!selectedFields.length) {
                setError(null);
                setEmpty(true);
            } else
                dispatch(
                    editRepresentationRequest({
                        key: gridName,
                        name,
                        isDefault: true,
                        oldName: representationName,
                        value: selectedFields,

                        callbackSuccess: () => {
                            onClose();
                        },
                    }),
                );
        }
        else {
            if (!name) {
                setError('required_field');
            } else if (isNotUniqueName() && name !== representationName) {
                setError('representation_already_exists');
            } else if (!selectedFields.length) {
                setError(null);
                setEmpty(true);
            } else {
                dispatch(
                    editRepresentationRequest({
                        key: gridName,
                        name,
                        isDefault: false,
                        oldName: representationName,
                        value: selectedFields,

                        callbackSuccess: () => {
                            onClose(name);
                        },
                    }),
                );
            }
        }

    };

    const handleDelete = () => {
        showConfirmation(t('confirm_delete_representation'), () => {
            dispatch(
                deleteRepresentationRequest({
                    key: gridName,
                    name: representationName,
                    callbackSuccess: () => {
                        onClose(() => {
                            changeRepresentation(null);
                        });
                    },
                }),
            );
        });
    };

    const closeConfirmation = () => {
        setConfirmation({open: false});
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
                        text={representationName || t('default_representation')}
                        fluid
                    >
                        <Dropdown.Menu>
                            <Dropdown.Item
                                text={t('default_representation')}
                                onClick={() => {
                                    changeRepresentation(null, true);
                                    setIsDefault(true)
                                }}
                            />
                            {representations && Object.keys(representations).length ? (
                                <>
                                    {Object.keys(representations)
                                        .sort()
                                        .map(key => (
                                            <Dropdown.Item
                                                text={key}
                                                onClick={() => {
                                                    changeRepresentation(key, true);
                                                    setIsDefault(false);
                                                }}
                                            />
                                        ))}
                                </>
                            ) : null}
                            <Dropdown.Divider/>
                            <Dropdown.Item icon="add" text={t('create_btn')} onClick={newOpen}/>
                        </Dropdown.Menu>
                    </Dropdown>
                </Form.Field>
                <Popup
                    content={t('customize_representation')}
                    position="bottom right"
                    trigger={
                        <Button icon="cogs" disabled={!representationName && !isEditDefaultRepresentation}
                                onClick={editOpen}/>
                    }
                />
            </div>
            <Modal
                dimmer="blurring"
                id="fieldModal"
                open={modalOpen}
                onClose={onClose}
                className="representation-modal"
                closeIcon
            >
                <Modal.Header>
                    {!isNew
                        ? t('Edit representation', {name: representationName})
                        : t('Create representation')}
                </Modal.Header>
                <Modal.Content scrolling>
                    <Modal.Description>
                        <Form style={{marginBottom: '16px'}}>

                            <Text
                                name="name"
                                value={name}
                                error={error && t(error)}
                                isDisabled={isDefault}
                                onChange={(e, {value}) => setName(value)}
                            />
                            <Input
                                icon="search"
                                iconPosition="left"
                                placeholder={t('search_field')}
                                value={search}
                                onChange={(e, {value}) => setSearch(value)}
                            />
                        </Form>
                        <div className="flex-container-justify">
                            {(fieldsList && fieldsList.length > 0) || (selectedFields && selectedFields.length) ? (
                                <DragAndDrop
                                    key={'dnd' + gridName}
                                    type={gridName}
                                    left={fieldsList}
                                    right={selectedFields}
                                    search={search}
                                    t={t}
                                    onChange={onChange}
                                />
                            ) : null}
                        </div>
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
                        <Button color="blue" onClick={!isNew ? handleEdit : handleSave}>
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
