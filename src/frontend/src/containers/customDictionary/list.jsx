import React from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

import TableInfo from '../../components/TableInfo';
import {
    canCreateByFormSelector,
    canExportToExcelSelector,
    canImportFromExcelSelector,
    columnsSelector,
    exportProgressSelector,
    exportToExcelRequest,
    getListRequest,
    importFromExcelRequest,
    importProgressSelector,
    listSelector,
    progressSelector,
    totalCountSelector,
} from '../../ducks/dictionaryView';
import { Button, Icon } from 'semantic-ui-react';
import Card from './card';

const newModal = (t, load, name) => (
    <Card title={t('createCard', { name: t(name) })} id={null} loadList={load} name={name}>
        <Button size="small" color="blue" className="grid-action-btn">
            <Icon name="plus" /> {t('create_btn')}
        </Button>
    </Card>
);

const List = ({
    match = {},
    columns,
    loadList,
    progress,
    totalCount,
    list,
    isCreateBtn,
    isImportBtn,
    isExportBtn,
    importFromExcel,
    exportFromExcel,
    importLoader,
    exportLoader,
}) => {
    const { params = {} } = match;
    const { name = '' } = params;
    const { t } = useTranslation();

    const handleImportFromExcel = (form, callbackSuccess) => {
        importFromExcel({
            form,
            name,
            callbackSuccess,
        });
    };

    const handleExportToExcel = () => {
        exportFromExcel({
            name,
        });
    };

    return (
        <TableInfo
            headerRow={columns}
            name={name}
            className="wider container-margin-top-bottom"
            loadList={loadList}
            loading={progress}
            totalCount={totalCount}
            title={name}
            list={list}
            isImportBtn={isImportBtn}
            isExportBtn={isExportBtn}
            importFromExcel={handleImportFromExcel}
            exportToExcel={handleExportToExcel}
            importLoader={importLoader}
            exportLoader={exportLoader}
            newModal={isCreateBtn ? newModal : null}
            modalCard={isCreateBtn ? <Card title={t('editCard', { name: t(name) })} /> : null}
        />
    );
};

const mapStateToProps = (state, ownProps) => {
    const { match = {} } = ownProps;
    const { params = {} } = match;
    const { name = '' } = params;

    return {
        columns: columnsSelector(state, name),
        progress: progressSelector(state),
        totalCount: totalCountSelector(state),
        list: listSelector(state),
        isCreateBtn: canCreateByFormSelector(state, name),
        isImportBtn: canImportFromExcelSelector(state, name),
        isExportBtn: canExportToExcelSelector(state, name),
        importLoader: importProgressSelector(state),
        exportLoader: exportProgressSelector(state),
    };
};

const mapDispatchToProps = dispatch => {
    return {
        loadList: params => {
            dispatch(getListRequest(params));
        },
        importFromExcel: params => {
            dispatch(importFromExcelRequest(params));
        },
        exportFromExcel: params => {
            dispatch(exportToExcelRequest(params));
        },
    };
};

export default withRouter(
    connect(
        mapStateToProps,
        mapDispatchToProps,
    )(List),
);
