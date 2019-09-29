import React, { useRef } from 'react';
import { Button, Form, Grid, Popup } from 'semantic-ui-react';
import Search from '../../../components/Search';
import { useTranslation } from 'react-i18next';
import { useSelector, useDispatch } from 'react-redux';
import Select from '../../BaseComponents/Select';
import {
    canImportFromExcelSelector, exportProgressSelector,
    exportToExcelRequest,
    importFromExcelRequest,
    importProgressSelector
} from "../../../ducks/gridList";

const Header = ({
    createButton,
    searchValue,
    searchOnChange,
    counter,
    clearFilter,
    disabledClearFilter,
    loadList,
    name,
}) => {
    const { t } = useTranslation();

    const dispatch = useDispatch();

    const fileUploader = useRef(null);

    const isImportBtn = useSelector(state => canImportFromExcelSelector(state, name));

    const importLoader = useSelector(state => importProgressSelector(state));
    const exportLoader = useSelector(state => exportProgressSelector(state));

    const exportExcel = () => {

        dispatch(exportToExcelRequest({name}))
    };

    const importExcel = () => {
        fileUploader && fileUploader.current.click();
    };

    const onFilePicked = e => {
        const file = e.target.files[0];

        const data = new FormData();
        data.append('FileName', file.name);
        data.append('FileContent', new Blob([file], { type: file.type }));
        data.append('FileContentType', file.type);

        dispatch(importFromExcelRequest({
            name,
            form: data,
            callbackSuccess: () => loadList()
        }))
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
                        onInput={onFilePicked}
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
                            trigger={<Button icon="upload" loading={importLoader} onClick={importExcel} />}
                        />
                    )}
                    {true && ( // todo
                        <Popup
                            content={t('exportToExcel')}
                            position="bottom right"
                            trigger={<Button icon="download" loading={exportLoader} onClick={exportExcel} />}
                        />
                    )}
                </Grid.Column>
            </Grid.Row>
        </Grid>
    );
};

export default Header;
