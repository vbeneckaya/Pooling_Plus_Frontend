import React, {useRef} from 'react';
import {Button, Grid, Popup} from 'semantic-ui-react';
import Search from '../../../components/Search';
import {useTranslation} from 'react-i18next';
import {useDispatch, useSelector} from 'react-redux';
import {
    canExportToExcelSelector,
    canImportFromExcelSelector,
    exportProgressSelector,
    exportToExcelRequest,
    exportFormsToExcelRequest,
    importFromExcelRequest,
    importFormsFromExcelRequest,
    importProgressSelector,
} from '../../../ducks/gridList';
import {canEditFieldPropertiesSelector} from '../../../ducks/profile';
import FieldsConfig from './representations';
import {
    getRepresentationsRequest,
    representationsSelector,
    setRepresentationRequest,
} from '../../../ducks/representations';
import Icon from '../../CustomIcon';
import {GRID_CARD_LINK} from "../../../router/links";
import {ORDERS_GRID, SHIPPINGS_GRID} from "../../../constants/grids";

const Header = ({
                    isCreateBtn,
                    searchValue,
                    searchOnChange,
                    counter,
                    clearFilter,
                    disabledClearFilter,
                    loadList,
                    name,
                    setSelected,
                    representationName,
                    filter,
                    goToCard,
                }) => {
    const {t} = useTranslation();

    const dispatch = useDispatch();

    const fileUploader = useRef(null);

    const isImportBtn = useSelector(state => canImportFromExcelSelector(state, name));
    const isExportBtn = useSelector(state => canExportToExcelSelector(state, name));

    const importLoader = useSelector(state => importProgressSelector(state));
    const exportLoader = useSelector(state => exportProgressSelector(state));

    const representations = useSelector(state => representationsSelector(state, name));
    const isEditDefaultRepresentation = useSelector(state => canEditFieldPropertiesSelector(state))

    const exportExcel = () => {
        if (name == SHIPPINGS_GRID)
            dispatch(exportFormsToExcelRequest({name, nameInner: ORDERS_GRID, filter: filter.filter}));

        else
            dispatch(exportToExcelRequest({name, filter: filter.filter}));
    };

    const importExcel = () => {
        fileUploader && fileUploader.current.click();
    };

    const onFilePicked = e => {
        const file = e.target.files[0];

        const data = new FormData();
        data.append('FileName', file.name);
        data.append('FileContent', new Blob([file], {type: file.type}));
        data.append('FileContentType', file.type);
        if (name == SHIPPINGS_GRID)
            dispatch(
                importFormsFromExcelRequest({
                    name,
                    form: data,
                    callbackSuccess: () => loadList(false, true),
                }),
            );
        else
            dispatch(
                importFromExcelRequest({
                    name,
                    form: data,
                    callbackSuccess: () => loadList(false, true),
                }),
            );
    };

    const getRepresentations = (payload) => {
        const {representationToSetName} = payload;
        dispatch(getRepresentationsRequest({key: name, representationToSetName: representationToSetName}));
    };

    const changeRepresentation = (key, isEdit) => {
        dispatch(
            setRepresentationRequest({
                gridName: name,
                value: key,
                callbackSuccess: () => {
                    setSelected(new Set());
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
                <Grid.Column width={5}>
                    <FieldsConfig
                        gridName={name}
                        representationName={representationName}
                        getRepresentations={getRepresentations}
                        changeRepresentation={changeRepresentation}
                        representations={representations}
                        isEditDefaultRepresentation={isEditDefaultRepresentation}
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
                                <Icon name="clear-filter" color={disabledClearFilter ? undefined : "#18a8cc"}/>
                            </Button>
                        }
                    />
                    <Search
                        searchValue={searchValue}
                        className="search-input"
                        value={filter.filter.filter.search}
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
