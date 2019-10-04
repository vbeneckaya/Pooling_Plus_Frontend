import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import {useSelector} from 'react-redux';
import {Button, Form, Modal} from 'semantic-ui-react';
import Text from "../BaseComponents/Text";
import DragAndDropFields from "./DragAndDropFields";
import {columnsGridSelector} from "../../ducks/gridList";

const FieldsConfig = ({ children, title, gridName }) => {
    let [modalOpen, setModalOpen ]= useState(false);
    let [selectedFields, setSelectedFields] = useState([]);

    let [name, setName] = useState('');

    const { t } = useTranslation();

    const fieldsList = useSelector(state => columnsGridSelector(state, gridName));

    const onOpen = () => {
        setModalOpen(true);
    };

    const onClose = () => {
        setModalOpen(false);
    };

    const onChange = (selected) => {
        setSelectedFields(selected);
    };

    const handleSave = () => {

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
                    <Form style={{marginBottom: "16px"}}>
                        <Text name="name" value={name} onChange={(e, {value}) => setName(value)}/>
                    </Form>
                    <DragAndDropFields
                        type={gridName}
                        fieldsConfig={selectedFields}
                        fieldsList={fieldsList}
                        onChange={onChange}
                    />
                </Modal.Description>
            </Modal.Content>
            <Modal.Actions>
                <Button color="grey" onClick={onClose}>
                    {t('CancelButton')}
                </Button>
                <Button color="blue" onClick={handleSave}>
                    {t('SaveButton')}
                </Button>
            </Modal.Actions>
        </Modal>
    );
};

export default FieldsConfig;
