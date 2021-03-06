import React, {Component} from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import {withTranslation} from 'react-i18next';

import TableInfo from '../../components/TableInfo';
import {
    canCreateByFormSelector,
    canExportToExcelSelector,
    canImportFromExcelSelector,
    clearDictionaryInfo,
    columnsSelector, descriptionSelector,
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
import {DICTIONARY_CARD_LINK, DICTIONARY_NEW_LINK} from '../../router/links';

const newModal = (t, load, name) => (
    <Card title={`${t(name)}: ${t('new_record')}`} id={null} loadList={load} name={name}>
        <Button icon="add" />
    </Card>
);

class List extends Component {
    state = {};

    componentWillUnmount() {
        this.props.clear();
    }

    handleImportFromExcel = (form, callbackSuccess) => {
        const {importFromExcel, match} = this.props;
        const {params = {}} = match;
        const {name = ''} = params;

        importFromExcel({
            form,
            name,
            callbackSuccess,
        });
    };

    handleExportToExcel = filter => {
        const {exportFromExcel, match} = this.props;
        const {params = {}} = match;
        const {name = ''} = params;
        exportFromExcel({
            name,
            filter,
        });
    };

    getCard = ({row, loadList, name}) => {
        const { t, isCreateBtn } = this.props;

        return isCreateBtn ? (
            <Card title={`${t(name)}: ${t('edit_record')}`} loadList={loadList} id={row.id}/>
        ) : null;
    };

    render() {
        const {
            match = {},
            columns,
            loadList,
            progress,
            totalCount,
            list,
            isCreateBtn,
            isImportBtn,
            isExportBtn,
            importLoader,
            exportLoader,
            clear,
            t,
            description
        } = this.props;
        const {params = {}} = match;
        const {name = ''} = params;

        return (
            <TableInfo
                key={name}
                headerRow={columns}
                name={name}
                className={
                    columns.length >= 10
                        ? 'container'
                        : 'wider ui container container-margin-top-bottom'
                }
                loadList={loadList}
                loading={progress}
                totalCount={totalCount}
                title={name}
                list={list}
                clear={clear}
                description={description}
                isImportBtn={isImportBtn}
                isExportBtn={isExportBtn}
                importFromExcel={this.handleImportFromExcel}
                exportToExcel={this.handleExportToExcel}
                importLoader={importLoader}
                exportLoader={exportLoader}
                newLink={isCreateBtn ? DICTIONARY_NEW_LINK : null}
                cardLink={DICTIONARY_CARD_LINK}
            />
        );
    }
}

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
        description: descriptionSelector(state, name)
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
        clear: () => {
            dispatch(clearDictionaryInfo());
        },
    };
};

export default withTranslation()(
    withRouter(
        connect(
            mapStateToProps,
            mapDispatchToProps,
        )(List),
    ),
);
