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
    <Card title={`${t(name)}: ${t('new_record')}`} id={null} loadList={load} name={name}>
        <Button size="small" color="blue" className="grid-action-btn">
            <Icon name="plus" /> {t('create_btn')}
        </Button>
    </Card>
);

class List extends Component {

    componentDidMount() {
        console.log('dictionary');
    }

    componentWillUnmount() {
        console.log('clear dictionary');
    }

    handleImportFromExcel = (form, callbackSuccess) => {
        const {importFromExcel, name} = this.props;

        importFromExcel({
            form,
            name,
            callbackSuccess,
        });
    };

    handleExportToExcel = () => {
        const {exportFromExcel, name} = this.props;
        exportFromExcel({
            name,
        });
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
        } = this.props;
        const {params = {}} = match;
        const {name = ''} = params;

        return (
            <TableInfo
                key={name}
                headerRow={columns}
                name={name}
                className="wider container-margin-top-bottom"
                loadList={loadList}
                loading={progress}
                totalCount={totalCount}
                title={name}
                list={list}
                clear={clear}
                isImportBtn={isImportBtn}
                isExportBtn={isExportBtn}
                importFromExcel={this.handleImportFromExcel}
                exportToExcel={this.handleExportToExcel}
                importLoader={importLoader}
                exportLoader={exportLoader}
                newModal={isCreateBtn ? newModal : null}
                modalCard={isCreateBtn ? <Card title={`${t(name)}: ${t('edit_record')}`}/> : null}
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
