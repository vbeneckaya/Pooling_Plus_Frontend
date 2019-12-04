import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import {Button, Container, Dimmer, Grid, Icon, Loader, Popup, Table} from 'semantic-ui-react';
import InfiniteScrollTable from '../InfiniteScrollTable';
import { debounce } from 'throttle-debounce';
import { PAGE_SIZE } from '../../constants/settings';
import Search from '../Search';
import './style.scss';
import CellValue from '../ColumnsValue';
import { withTranslation } from 'react-i18next';
import HeaderCellComponent from "./components/header-cell";
import BodyCellComponent from "./components/body-cell";
import _ from "lodash";
import BodyCell from "../SuperGrid/components/body_cell";

const ModalComponent = ({ element, props, children }) => {
    if (!element) {
        return <>{children}</>;
    }
    return React.cloneElement(element, props, children);
};

class TableInfo extends Component {
    state = {
        page: 1,
        filter: '',
    };

    componentDidMount() {
        this.load();
    }

    componentDidUpdate(prevProps) {
        if (this.props.name !== prevProps.name) {
            console.log('listupdate')
        }
    }

    mapData = (isConcat, isReload) => {
        const { filter, page } = this.state;
        const { name } = this.props;

        const params = {
            filter: {
                search: filter,
                take: isReload ? page * PAGE_SIZE : PAGE_SIZE,
                skip: isReload ? 0 : (page - 1) * PAGE_SIZE,
            },
            isConcat,
            name,
        };

        return params;
    };

    load = (isConcat, isReload) => {
        const { loadList } = this.props;
        loadList(this.mapData(isConcat, isReload));
    };

    nextPage = () => {
        const { totalCount, list = [] } = this.props;

        if (list.length < totalCount) {
            this.setState(
                prevState => ({
                    page: prevState.page + 1,
                }),
                () => this.load(true),
            );
        }
    };

    changeFullTextFilter = (e, { value }) => {
        this.setState({filter: value, page: 1}, this.load);
    };

    debounceSetFilterApiAndLoadList = debounce(300, this.load);

    headerRowComponent = () => (
        <Table.Row>
            {this.props.headerRow &&
            this.props.headerRow.map((row, index) => (
                <HeaderCellComponent key={row.name} row={row}/>
                ))}
            {this.props.isShowActions ? <Table.HeaderCell /> : null}
        </Table.Row>
    );

    importFromExcel = () => {
        this.fileUploader && this.fileUploader.click();
    };

    exportToExcel = () => {
        this.props.exportToExcel && this.props.exportToExcel(this.mapData())
    };

    onFilePicked = e => {
        const file = e.target.files[0];

        const data = new FormData();
        data.append('FileName', file.name);
        data.append('FileContent', new Blob([file], { type: file.type }));
        data.append('FileContentType', file.type);
        this.props.importFromExcel(data, () => this.load(false, true));

        e.target.value = null;
    };

    handleToggleIsActive = (
        event,
        {itemID, checked},
    ) => {
        this.props.toggleIsActive(
            event,
            {itemID, checked},
            this.load,
        )
    };


    render() {
        const {
            headerRow,
            className,
            list = [],
            title,
            isShowActions,
            actions,
            loading,
            customRowComponent,
            groupActions,
            toggleIsActive,
            newModal,
            t,
            name,
            modalCard,
            isImportBtn,
            isExportBtn,
            importLoader,
            exportLoader,
            exportToExcel,
            totalCount
        } = this.props;

        const { filter } = this.state;

        return (
            <div className={className}>
                <Loader active={loading && !list.length} size="huge" className="table-loader">Loading</Loader>
                    <Grid className="table-header-menu">
                        <Grid.Row>
                            {/*<Grid.Column width={7}>
                                <Search
                                    value={filter}
                                    className="search-input"
                                    onChange={this.changeFullTextFilter}
                                />
                                <span className="records-counter">{t('totalCount', {count: totalCount})}</span>
                            </Grid.Column>
                            <Grid.Column width={9} textAlign="right">
                                <input
                                    type="file"
                                    ref={instance => {
                                        this.fileUploader = instance;
                                    }}
                                    style={{ display: 'none' }}
                                    onChange={this.onFilePicked}
                                />

                                {isImportBtn ? (
                                    <Button
                                        color="green"
                                        loading={importLoader}
                                        onClick={this.importFromExcel}
                                    >
                                        <Icon name="upload" />
                                        {t('importFromExcel')}
                                    </Button>
                                ) : null}
                                {isExportBtn ? (
                                    <Button
                                        color="green"
                                        loading={exportLoader}
                                        onClick={this.exportToExcel}
                                    >
                                        <Icon name="download" />
                                        {t('exportToExcel')}
                                    </Button>
                                ) : null}
                                {newModal ? newModal(t, this.load, name) : null}
                                {groupActions &&
                                    groupActions().map(action => {
                                        return (
                                            action.visible && (
                                                <Button
                                                    color={action.color}
                                                    loading={action.loading}
                                                    onClick={action.action}
                                                >
                                                    {action.buttonName}
                                                </Button>
                                            )
                                        );
                                    })}
                            </Grid.Column>*/}
                            <Grid.Column width={5} verticalAlign="middle">
                                <span className="table-header-menu_title">
                                    {t(name)}
                                </span>
                                <span className="records-counter">{t('totalCount', {count: totalCount})}</span>
                            </Grid.Column>
                            <Grid.Column width={11} textAlign="right">
                                {newModal ? newModal(t, this.load, name) : null}
                                {isImportBtn ? (
                                    <Popup
                                        content={t('importFromExcel')}
                                        position="bottom right"
                                        trigger={
                                            <Button
                                                icon="upload"
                                                loading={importLoader}
                                                onClick={this.importFromExcel}
                                            />
                                        }
                                    />
                                ) : null}
                                {isExportBtn ? (
                                    <Popup
                                        content={
                                            t('exportToExcel') // todo
                                        }
                                        position="bottom right"
                                        trigger={
                                            <Button
                                                icon="download"
                                                loading={exportLoader}
                                                onClick={this.exportToExcel}
                                            />
                                        }
                                    />
                                ) : null}
                                <Search
                                    value={filter}
                                    className="search-input"
                                    onChange={this.changeFullTextFilter}
                                />
                            </Grid.Column>
                        </Grid.Row>
                        <input
                            type="file"
                            ref={instance => {
                                this.fileUploader = instance;
                            }}
                            style={{ display: 'none' }}
                            onChange={this.onFilePicked}
                        />
                    </Grid>
                <div
                    className={`scroll-table-container`}
                    ref={instance => {
                        this.container = instance;
                    }}
                >
                    <InfiniteScrollTable
                        className="grid-table table-info"
                        onBottomVisible={this.nextPage}
                        context={this.container}
                        headerRow={this.headerRowComponent()}
                    >
                        <Table.Body>
                            {customRowComponent
                                ? customRowComponent
                                : list &&
                                  list.map((row, i) => (
                                      <ModalComponent
                                          element={modalCard}
                                          props={{ id: row.id, loadList: this.load, name }}
                                          key={`modal_${row.id}`}
                                      >
                                          <Table.Row key={row.id}>
                                              {headerRow.map((column, index) => (
                                                  <BodyCellComponent
                                                      key={`cell_${row.id}_${column.name}_${index}`}
                                                      column={column}
                                                      value={row[column.name] && typeof row[column.name] === 'object' ? row[column.name].value : row[column.name]}
                                                      valueText={row[column.name] && typeof row[column.name] === 'object' ? row[column.name].name : row[column.name]}
                                                      id={row.id}
                                                      toggleIsActive={this.handleToggleIsActive}
                                                      indexRow={i}
                                                      indexColumn={index}
                                                      t={t}
                                                  />
                                              ))}
                                              {isShowActions ? (
                                                  <Table.Cell textAlign="center">
                                                      {actions &&
                                                          actions(row, this.load, t).map(
                                                              (action, index) => (
                                                                  <React.Fragment
                                                                      key={`action_${index}`}
                                                                  >
                                                                      {action}
                                                                  </React.Fragment>
                                                              ),
                                                          )}
                                                  </Table.Cell>
                                              ) : null}
                                          </Table.Row>
                                      </ModalComponent>
                                  ))}
                            <div className="table-bottom-loader">
                                <Loader active={loading && list.length} />
                            </div>
                        </Table.Body>
                    </InfiniteScrollTable>
                </div>
            </div>
        );
    }
}

TableInfo.defaultProps = {
    loadList: () => {},
};

export default withTranslation()(TableInfo);
