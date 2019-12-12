import React from 'react';
import { Button, Icon, Modal } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';
import './style.scss';

const ConfirmDialog = ({ content, open, onNoClick, onYesClick, onCancelClick }) => {
    const { t } = useTranslation();

    const handleYesClick = () => {onYesClick()};

    const handleNoClick = () => {onNoClick()};

    const handleCancelClick = () => {onCancelClick()};

    return (
        <Modal open={open} basic size="small">
            {/*<Header icon='archive' content='Archive Old Messages' />*/}
            <Modal.Content>
                <p>{content}</p>
            </Modal.Content>
            <Modal.Actions className="confirm-dialog-actions">
                <div>
                    {onCancelClick && (
                        <Button basic color="grey" inverted onClick={handleCancelClick}>
                            {t('cancelConfirm')}
                        </Button>
                    )}
                </div>
                <div>
                    <Button basic color="red" inverted onClick={handleNoClick}>
                        <Icon name="remove" /> {t('No')}
                    </Button>
                    <Button color="green" inverted onClick={handleYesClick}>
                        <Icon name="checkmark" /> {t('Yes')}
                    </Button>
                </div>
            </Modal.Actions>
        </Modal>
    );
};

export default ConfirmDialog;
