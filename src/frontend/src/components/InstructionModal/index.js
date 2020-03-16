import React from 'react';
import {Button, Icon, Modal} from 'semantic-ui-react';
import {useTranslation} from 'react-i18next';
import {useDispatch, useSelector} from "react-redux";
import {showInstructionSelector, hideInstruction} from "../../ducks/general";

const InstructionModal = ({pathName, open, onNoClick, onYesClick, onCancelClick}) => {
    const {t} = useTranslation();

    const dispatch = useDispatch();

    const content = useSelector(state => showInstructionSelector(state));

    const handleYesClick = () => {
        onYesClick && onYesClick();
        dispatch(hideInstruction);
    };

    return (
        <Modal open={!!content} size="small" dimmer="blurring">
            {/*<Header icon='archive' content='Archive Old Messages' />*/}
            <Modal.Content>
                <p>{!!content && content}</p>
            </Modal.Content>
            <Modal.Actions className="confirm-dialog-actions">
                <div>
                    <Button color="blue" onClick={handleYesClick}>
                        <Icon name="checkmark"/> {t('Yes')}
                    </Button>
                </div>
            </Modal.Actions>
        </Modal>
    );
};

export default InstructionModal;
