﻿import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { Button, Confirm, Form, Icon, Modal, Popup } from 'semantic-ui-react';
import FileUploader from './FileUploader';
import DocView from './DocView';
import WebCamUploader from './WebCamUploader';
import './style.scss';
import {
    addDocumentRequest,
    deleteDocumentRequest,
    documentTypesSelector, downloadDocumentRequest,
    editDocumentRequest,
    getDocumentTypesRequest,
} from '../../ducks/documents';

const DocWithEditor = ({
    okButtonText,
    titleText,
    document: currentDocument,
    gridName,
    cardId,
    getDocuments,
}) => {
    let [modalOpen, setModalOpen] = useState(false);
    let [confirmation, setConfirmation] = useState({ open: false });
    let [document, setDocument] = useState(
        currentDocument ? Object.assign({}, currentDocument) : null,
    );

    const { t } = useTranslation();
    const dispatch = useDispatch();

    const documentTypes = useSelector(state => documentTypesSelector(state));

    const handleOpen = () => {
        dispatch(getDocumentTypesRequest());
        setModalOpen(true);
    };

    const handleClose = () => {
        // setDocument(null);
        getDocuments();
        setModalOpen(false);
    };

    const handleSave = () => {
        console.log('document', document);
        if (document && document.fileId) {
            document.id
                ? dispatch(
                      editDocumentRequest({
                          gridName,
                          cardId,
                          document: {
                              id: document.id,
                              name: document.name,
                              fileId: document.fileId,
                              typeId: document.typeId,
                          },
                          id: document.id,
                          callbackSuccess: () => {
                              handleClose();
                          },
                      }),
                  )
                : dispatch(
                      addDocumentRequest({
                          gridName,
                          cardId,
                          document: {
                              name: document.name,
                              fileId: document.fileId,
                              typeId: document.typeId,
                          },
                          callbackSuccess: () => {
                              handleClose();
                              setDocument(null);
                          },
                      }),
                  );
        }
    };

    const handleEdit = () => {
        if (document && document.fileId) {
            dispatch(
                editDocumentRequest({
                    gridName,
                    cardId,
                    document: {
                        id: document.id,
                        name: document.name,
                        fileId: document.fileId,
                        typeId: document.typeId,
                    },
                    id: document.id,
                    callbackSuccess: () => {
                        handleClose();
                    },
                }),
            );
        }
    };

    const handleDownload = () => {
        dispatch(downloadDocumentRequest({id: document.fileId}))
    };

    const handleDelete = document => {
        setConfirmation({
            open: true,
            content: t('Delete document', { name: document.name }),
            cancel: () => {
                setConfirmation({ open: false });
            },
            confirm: () => {
                dispatch(
                    deleteDocumentRequest({
                        gridName,
                        cardId,
                        id: document.id,
                        callbackSuccess: () => {
                            getDocuments();
                            setConfirmation({ open: false });
                        },
                    }),
                );
            },
        });
    };

    const addFile = (id, name) => {
        if (id) {
            setDocument({
                ...document,
                fileId: id,
                name: name,
            });
        }
    };

    const handleTypeChange = (e, { value }) => {
        setDocument({
            ...document,
            typeId: value,
        });
    };

    const getFileName = () => {
        if (!document || !document.name) return '';
        const { name } = document;
        return name.lastIndexOf('.') !== -1 ? name.substr(0, name.lastIndexOf('.')) : '';
    };

    const saveFileName = (e, { value }) => {
        if (!value || !document || !document.name || document.name.lastIndexOf('.') === -1) return;
        const currentName = document.name;
        const extension = currentName.substr(currentName.lastIndexOf('.'));

        setDocument({
            ...document,
            name: value + extension,
        });
    };

    return (
        <DocView document={document}>
            {document ? (
                <div>
                    <Popup
                        content={t('delete')}
                        position="bottom right"
                        trigger={
                            <Icon
                                name="times"
                                className="uploaded-image-delete-button"
                                onClick={() => handleDelete(document)}
                            />
                        }
                    />
                    <Popup
                        content={t('edit')}
                        position="bottom right"
                        trigger={
                            <Icon
                                name="pencil alternate"
                                className="uploaded-image-edit-button"
                                onClick={handleOpen}
                            />
                        }
                    />
                    <Popup
                        content={t('download')}
                        position="bottom right"
                        trigger={
                            <Icon
                                name="download"
                                className="uploaded-image-download-button"
                                onClick={handleDownload}
                            />
                        }
                    />
                </div>
            ) : (
                <Icon name="plus" className="uploaded-image-add-button" onClick={handleOpen} />
            )}
            <Modal
                size="mini"
                open={modalOpen}
                closeOnEscape
                closeOnDimmerClick={false}
                onClose={handleClose}
            >
                <Modal.Header>{titleText}</Modal.Header>
                <Modal.Content>
                    <Form>
                        <FileUploader document={document} onChange={addFile} />
                        <WebCamUploader onChange={addFile}>
                            <Button size="small" floated="right" icon="photo" content="Webcam" />
                        </WebCamUploader>
                        <Form.Field>
                            <Form.Input
                                label={t('Document name')}
                                placeholder={t('Upload file')}
                                name="name"
                                value={getFileName()}
                                onChange={saveFileName}
                            />
                        </Form.Field>
                        <Form.Field>
                            <Form.Dropdown
                                label={t('Type')}
                                fluid
                                selection
                                name="documentType"
                                value={document ? document.typeId : null}
                                options={documentTypes}
                                onChange={handleTypeChange}
                            />
                        </Form.Field>
                    </Form>
                </Modal.Content>
                <Modal.Actions>
                    <Button icon="ban" content={t('CancelButton')} onClick={handleClose} />
                    <Button icon="check" positive content={okButtonText} onClick={handleSave} />
                </Modal.Actions>
            </Modal>
            <Confirm
                open={confirmation.open}
                content={confirmation.content}
                onCancel={confirmation.cancel}
                onConfirm={confirmation.confirm}
            />
        </DocView>
    );
};

export default DocWithEditor;
