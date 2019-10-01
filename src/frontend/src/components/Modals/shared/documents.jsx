import React, { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';
import DocWithEditor from '../../Documents/DocWithEditor';
import {documentsSelector, getDocumentsRequest} from "../../../ducks/documents";

const Documents = ({ gridName, cardId }) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();

    const getDocuments = () => {
        dispatch(getDocumentsRequest({gridName, cardId}))
    };

    useEffect(() => {
        getDocuments()
    }, []);

    const documents = useSelector(state => documentsSelector(state));


    return (
        <div className="flex-container">
            {documents.map((document, index) => (
                <DocWithEditor
                    key={document.fileId}
                    gridName={gridName}
                    cardId={cardId}
                    document={document}
                    getDocuments={getDocuments}
                    titleText={t('Edit document')}
                    okButtonText={t('SaveButton')}
                />
            ))}
            <DocWithEditor
                gridName={gridName}
                cardId={cardId}
                getDocuments={getDocuments}
                titleText={t('Add document')}
                okButtonText={t('AddButton')}
            />
        </div>
    );
};

export default Documents;
