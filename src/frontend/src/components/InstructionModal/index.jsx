import React from 'react';
import {Button, Icon, Modal, Header} from 'semantic-ui-react';
import {useTranslation} from 'react-i18next';
import {useDispatch, useSelector} from "react-redux";
import {showInstructionSelector, hideInstruction} from "../../ducks/profile";

const InstructionModal = () => {
    const {t} = useTranslation();

    const dispatch = useDispatch();

    const content = useSelector(state => showInstructionSelector(state));

    const handleYesClick = () => {
        debugger;
        //dispatch(hideInstruction());
    };

    return (
        <Modal open={!!content} size="small" dimmer="blurring"  closeIcon
               onClose={handleYesClick}
               closeOnDimmerClick={true}
        >
            <Header  content={!!content && content.title} />
            <Modal.Content>
                <p>{!!content && content.content}</p>
            </Modal.Content>
            <Modal.Actions className="confirm-dialog-actions">
                <div>
                    <Button color="blue" onClick={handleYesClick}>
                        <Icon name="checkmark"/> {t('Understand')}
                    </Button>
                </div>
            </Modal.Actions>
        </Modal>
    );
};

export default InstructionModal;
