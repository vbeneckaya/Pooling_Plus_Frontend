import React, { useEffect, useRef } from 'react';
import {Button, Grid, Popup} from 'semantic-ui-react';
import Search from '../../../components/Search';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import {
    canExportToExcelSelector,
    canImportFromExcelSelector,
    exportProgressSelector,
    exportToExcelRequest,
    importFromExcelRequest,
    importProgressSelector,
} from '../../../ducks/gridList';

import FieldsConfig from '../../Representations/index';
import {
    getRepresentationsRequest,
    representationsSelector,
    setRepresentationRequest,
} from '../../../ducks/representations';
import AllFilters from './all_filters';
import Icon from '../../CustomIcon';

const Header = ({
                    isCreateBtn,
    searchValue,
    searchOnChange,
    counter,
    clearFilter,
    updatingFilter,
    disabledClearFilter,
    loadList,
    name,
    setSelected,
    filter,
                    goToCard,
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
        dispatch(exportToExcelRequest({ name, filter: filter.filter }));
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
                callbackSuccess: () => loadList(false, true),
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

    const changeRepresentation = (key, isEdit) => {
        dispatch(
            setRepresentationRequest({
                gridName: name,
                value: key,
                callbackSuccess: () => {
                    setSelected(new Set());
                    !isEdit && clearFilter();
                },
            }),
        );
    };

    const handleGoToCard = () => {
        goToCard(false, null, name);
    };

    return (
        <Grid className="grid-header-panel">
            <Grid.Row>
                {/* <Grid.Column width={10}>
                    {createButton}
                    <Search searchValue={searchValue} className="search-input" onChange={searchOnChange}/>
                    <span className="records-counter">{t('totalCount', { count: counter })}</span>
                </Grid.Column>
                <Grid.Column width={6} className="grid-right-elements">
                    <input
                        type="file"
                        ref={fileUploader}
                        style={{ display: 'none' }}
                        onInput={onFilePicked}
                    />
                    <FieldsConfig
                        gridName={name}
                        getRepresentations={getRepresentations}
                        changeRepresentation={changeRepresentation}
                        representations={representations}
                    />
                    <Popup
                        content={t('reset_filters')}
                        position="bottom right"
                        trigger={
                            <Button
                                icon="clear-filter"
                                className={`clear-filter-btn`}
                                onClick={clearFilter}
                                disabled={disabledClearFilter}
                            />
                        }
                    />
                    {
                        <Popup
                            content={<AllFilters gridName={name} filter={filter}/>}
                            position="bottom right"
                            trigger={
                                <Button
                                    icon
                                >
                                    <Icon color="primary" name={"sliders horizontal"}/>
                                </Button>
                            }
                            on="click"
                        />
                    }
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
                </Grid.Column>*/}
                <Grid.Column width={5}>
                    <FieldsConfig
                        gridName={name}
                        getRepresentations={getRepresentations}
                        changeRepresentation={changeRepresentation}
                        representations={representations}
                    />
                </Grid.Column>
                <Grid.Column width={1} verticalAlign="middle">
                    <span className="records-counter">{t('totalCount', {count: counter})}</span>
                </Grid.Column>
                <Grid.Column width={10} className="grid-right-elements">
                    {isCreateBtn && (
                        <Popup
                            content={t('add_record')}
                            position="bottom right"
                            trigger={<Button icon="add" onClick={handleGoToCard}/>}
                        />
                    )}
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
                    <Popup
                        content={t('reset_filters')}
                        position="bottom right"
                        trigger={
                            <Button
                                icon
                                className={`clear-filter-btn`}
                                onClick={clearFilter}
                                disabled={disabledClearFilter}
                            >
                                <Icon name="clear-filter"/>
                            </Button>
                        }
                    />
                    <Search
                        searchValue={searchValue}
                        className="search-input"
                        onChange={searchOnChange}
                    />
                </Grid.Column>
            </Grid.Row>
            <input
                type="file"
                ref={fileUploader}
                style={{display: 'none'}}
                onInput={onFilePicked}
            />
        </Grid>
    );
};

export default Header;
