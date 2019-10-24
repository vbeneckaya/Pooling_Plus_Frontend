import React, { useRef, useState, useEffect } from 'react';
import { Button, Dropdown, Form, Grid, Popup } from 'semantic-ui-react';
import Search from '../../../components/Search';
import { useTranslation } from 'react-i18next';
import { useSelector, useDispatch } from 'react-redux';
import Select from '../../BaseComponents/Select';
import {
    canExportToExcelSelector,
    canImportFromExcelSelector,
    exportProgressSelector,
    exportToExcelRequest,
    importFromExcelRequest,
    importProgressSelector,
} from '../../../ducks/gridList';

import FieldsConfig from '../../FieldsConfig/index';
import {
    getRepresentationsRequest,
    representationNameSelector,
    representationsSelector,
    setRepresentationRequest,
} from '../../../ducks/representations';

const Header = ({
    createButton,
    searchValue,
    searchOnChange,
    counter,
    clearFilter,
    disabledClearFilter,
    loadList,
    name,
    setSelected,
    filter
}) => {
    const { t } = useTranslation();

    const dispatch = useDispatch();

    const fileUploader = useRef(null);

    const isImportBtn = useSelector(state => canImportFromExcelSelector(state, name));
    const isExportBtn = useSelector(state => canExportToExcelSelector(state, name));

    const importLoader = useSelector(state => importProgressSelector(state));
    const exportLoader = useSelector(state => exportProgressSelector(state));

    const representations = useSelector(state => representationsSelector(state, name));

    const exportExcel = () => {
        dispatch(exportToExcelRequest({ name, filter }));
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

        dispatch(
            importFromExcelRequest({
                name,
                form: data,
                callbackSuccess: () => loadList(),
            }),
        );
    };

    const getRepresentations = callBackFunc => {
        dispatch(getRepresentationsRequest({ key: name, callBackFunc }));
    };

    useEffect(
        () => {
            getRepresentations();
        },
        [name],
    );

    const representation = useSelector(state => representationNameSelector(state, name));

    const changeRepresentation = key => {
        dispatch(
            setRepresentationRequest({
                gridName: name,
                value: key,
                callbackSuccess: () => {
                    setSelected(new Set());
                    clearFilter();
                }
            }),
        );
    };

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
                        <Form.Field>
                            <Dropdown
                                selection
                                text={representation || t('default_representation')}
                                fluid
                            >
                                <Dropdown.Menu>
                                    <Dropdown.Item
                                        text={t('default_representation')}
                                        onClick={() => changeRepresentation(null)}
                                    />
                                    {representations && Object.keys(representations).length ? (
                                        <>
                                            {Object.keys(representations)
                                                .sort()
                                                .map(key => (
                                                    <Dropdown.Item
                                                        text={key}
                                                        onClick={() => changeRepresentation(key)}
                                                    />
                                                ))}
                                        </>
                                    ) : null}
                                    <Dropdown.Divider />
                                    <FieldsConfig
                                        title={t('Create representation')}
                                        gridName={name}
                                        isNew={true}
                                        getRepresentations={getRepresentations}
                                    >
                                        <Dropdown.Item icon="add" text={t('create_btn')} />
                                    </FieldsConfig>
                                </Dropdown.Menu>
                            </Dropdown>
                        </Form.Field>
                        <FieldsConfig
                            title={t('Edit representation', { name: representation })}
                            gridName={name}
                            getRepresentations={getRepresentations}
                        >
                            <Button icon="cogs" disabled={!representation} />
                        </FieldsConfig>
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
                            trigger={
                                <Button
                                    icon="upload"
                                    loading={importLoader}
                                    onClick={importExcel}
                                />
                            }
                        />
                    )}
                    {isExportBtn && (
                        <Popup
                            content={
                                t('exportToExcel') // todo
                            }
                            position="bottom right"
                            trigger={
                                <Button
                                    icon="download"
                                    loading={exportLoader}
                                    onClick={exportExcel}
                                />
                            }
                        />
                    )}
                </Grid.Column>
            </Grid.Row>
        </Grid>
    );
};

export default Header;
