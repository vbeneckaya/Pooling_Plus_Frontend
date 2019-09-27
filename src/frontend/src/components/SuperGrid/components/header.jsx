import React, { useRef } from 'react';
import { Button, Form, Grid, Popup } from 'semantic-ui-react';
import Search from '../../../components/Search';
import { useTranslation } from 'react-i18next';
import Select from '../../BaseComponents/Select';

const Header = ({
    createButton,
    searchValue,
    searchOnChange,
    counter,
    clearFilter,
    disabledClearFilter,
    isImportBtn,
    importFromExcel,
    exportToExcel,
}) => {
    const { t } = useTranslation();

    const fileUploader = useRef(null);

    const exportExcel = () => {exportToExcel()};

    const importExcel = () => {
        fileUploader && fileUploader.current.click();
    };

    const onFilePicked = e => {
        const file = e.target.files[0];

        const data = new FormData();
        data.append('FileName', file.name);
        data.append('FileContent', new Blob([file], { type: file.type }));
        data.append('FileContentType', file.type);
        importFromExcel(data);
    };

    const settings = () => {};

    return (
        <Grid className="grid-header-panel">
            <Grid.Row>
                <Grid.Column width={10}>
                    {createButton}
                    <Search value={searchValue} onChange={searchOnChange} isAuto />
                    <span className="records-counter">{t('totalCount', { count: counter })}</span>
                </Grid.Column>
                <Grid.Column width={6} className="grid-right-elements">
                    <input
                        type="file"
                        ref={fileUploader}
                        style={{ display: 'none' }}
                        onChange={onFilePicked}
                    />
                    <div className="representation">
                        <label>{t('representation')}</label>
                        <Select />
                        <Button icon="cogs" onClick={settings} />
                    </div>
                    <Popup
                        content={t('reset_filters')}
                        position="bottom right"
                        trigger={
                            <Button
                                icon="times"
                                onClick={clearFilter}
                                disabled={disabledClearFilter}
                            />
                        }
                    />
                    {isImportBtn && (
                        <Popup
                            content={t('importFromExcel')}
                            position="bottom right"
                            trigger={<Button icon="upload" onClick={importExcel} />}
                        />
                    )}
                    {true && ( // todo
                        <Popup
                            content={t('exportToExcel')}
                            position="bottom right"
                            trigger={<Button icon="download" onClick={exportExcel} />}
                        />
                    )}
                </Grid.Column>
            </Grid.Row>
        </Grid>
    );
};

export default Header;
