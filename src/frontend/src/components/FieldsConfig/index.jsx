import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import {Button, Form, Input, Modal, Search} from 'semantic-ui-react';
import Text from '../BaseComponents/Text';
import DragAndDropFields from './DragAndDropFields';
import { columnsGridSelector } from '../../ducks/gridList';
import {
    editRepresentationRequest,
    representationNameSelector,
    representationSelector,
    saveRepresentationRequest,
    setRepresentationRequest,
} from '../../ducks/representations';


const FieldsConfig = ({ children, title, gridName, isNew }) => {
    const currName = useSelector(state => representationNameSelector(state, gridName));
    const currRepresentation = useSelector(state => representationSelector(state, gridName)) || [];

    let [modalOpen, setModalOpen] = useState(false);
    let [selectedFields, setSelectedFields] = useState(isNew ? [] : currRepresentation);
    let [name, setName] = useState(isNew ? '' : currName);
    let [error, setError] = useState(false);
    let [search, setSearch] = useState('');

    const { t } = useTranslation();
    const dispatch = useDispatch();

    const fieldsList = useSelector(state => columnsGridSelector(state, gridName)).filter(column => {
        return !selectedFields.map(item => item.name).includes(column.name);
    });

    useEffect(
        () => {
            !isNew && setName(currName);
        },
        [currName],
    );
    useEffect(
        () => {
            !isNew && setSelectedFields(currRepresentation);
        },
        [currRepresentation],
    );


    const onOpen = () => {
        setModalOpen(true);
    };

    const onClose = () => {
        setModalOpen(false);
        setSearch('');
    };

    const onChange = selected => {
        setSelectedFields(selected);
    };

    const handleSave = () => {
        if (!name) {
            setError(true);
        } else {
            dispatch(
                saveRepresentationRequest({
                    gridName,
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
            setError(true);
        } else {
            dispatch(
                editRepresentationRequest({
                    gridName,
                    name,
                    oldName: currName,
                    value: selectedFields,
                    callbackSuccess: () => {
                        dispatch(
                            setRepresentationRequest({
                                gridName,
                                value: name,
                            }),
                        );
                        onClose();
                    },
                }),
            );
        }
    };

    const fieldsListSearch = fieldsList.filter(item => t(item.name).toLowerCase().includes(search.toLowerCase()));
    const selectedListSearch = selectedFields.filter(item => t(item.name).toLowerCase().includes(search.toLowerCase()));

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
                            onChange={(e, { value }) => setName(value)}
                        />
                        <Input
                            icon='search'
                            iconPosition='left'
                            placeholder='Поиск поля'
                            value={search}
                            clearable
                            onChange={(e, {value}) => setSearch(value)}
                        />
                    </Form>
                    <DragAndDropFields
                        type={gridName}
                        fieldsConfig={selectedListSearch}
                        fieldsList={fieldsListSearch}
                        onChange={onChange}
                    />
                </Modal.Description>
            </Modal.Content>
            <Modal.Actions>
                <Button color="grey" onClick={onClose}>
                    {t('CancelButton')}
                </Button>
                <Button color="blue" onClick={isNew ? handleSave : handleEdit}>
                    {t('SaveButton')}
                </Button>
            </Modal.Actions>
        </Modal>
    );
};

export default FieldsConfig;
