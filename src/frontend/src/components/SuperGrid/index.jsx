import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { debounce } from 'throttle-debounce';

import './style.scss';
import Filter from './components/filter';
import HeaderSearchGrid from './components/header';
import InfiniteScrollTable from '../InfiniteScrollTable';

import Result from './components/result';
import { PAGE_SIZE } from '../../constants/settings';
import { Button, Confirm, Grid, Dimmer, Loader } from 'semantic-ui-react';

const initState = (storageFilterItem, storageSortItem) => ({
    page: 1,
    filters: storageFilterItem ? JSON.parse(localStorage.getItem(storageFilterItem)) || {} : {},
    sort: storageSortItem ? JSON.parse(localStorage.getItem(storageSortItem)) || {} : {},
    fullText: '',
    selectedRows: new Set(),
});

const CreateButton = ({ button, ...res }) => {
    return React.cloneElement(button, res);
};

class SuperGrid extends Component {
    constructor(props) {
        super(props);

        const { storageFilterItem, storageSortItem } = props;

        this.state = {
            ...initState(storageFilterItem, storageSortItem),
        };
    }

    componentDidMount() {
        this.props.autoUpdateStart(this.mapData());
    }

    componentWillUnmount() {
        this.props.autoUpdateStop();
        this.props.clearStore();
    }

    componentDidUpdate(prevProps) {
        const { selectedRows } = this.state;
        const newSelectedRow = new Set(selectedRows);

        if (
            selectedRows.size &&
            selectedRows.size !== this.props.totalCount &&
            prevProps.rows !== this.props.rows
        ) {
            const rowsIds = this.props.rows.map(item => item.id);

            for (let item of selectedRows) {
                if (!rowsIds.includes(item)) {
                    newSelectedRow.delete(item);
                }
            }

            this.setSelected(newSelectedRow);
        }

        if (this.props.name !== prevProps.name) {
            console.log('!!!!', 1111);
            const {
                autoUpdateStop,
                clearStore,
                storageFilterItem,
                storageSortItem,
                autoUpdateStart,
            } = this.props;

            autoUpdateStop();
            clearStore();
            this.setState(
                {
                    ...initState(storageFilterItem, storageSortItem),
                },
                autoUpdateStart(this.mapData()),
            );
        }
    }

    mapData = (isConcat, isReload) => {
        const { filters, page, sort, fullText } = this.state;
        const { extParams, defaultFilter, name } = this.props;

        let params = {
            filter: {
                filter: {
                    ...filters,
                    search: fullText,
                    ...defaultFilter,
                },
                take: isReload ? page * PAGE_SIZE : PAGE_SIZE,
                skip: isReload ? 0 : (page - 1) * PAGE_SIZE,
                sort,
            },
            ...extParams,
            name,
            isConcat,
        };

        return params;
    };

    loadList = (isConcat, isReload) => {
        const { autoUpdateStop, autoUpdateStart, extGrid, loadList } = this.props;

        if (extGrid) {
            loadList(this.mapData(isConcat, isReload));
        } else {
            autoUpdateStop();
            autoUpdateStart(this.mapData(isConcat, isReload));
        }
    };

    nextPage = () => {
        const { totalCount, rows = [] } = this.props;

        if (rows.length < totalCount) {
            this.setState(
                prevState => ({
                    page: prevState.page + 1,
                }),
                () => this.loadList(true),
            );
        }
    };

    setFilter = (e, { name, value }) => {
        this.setState(prevState => {
            let { filters } = prevState;
            if (value) filters = { ...filters, [name]: value };
            else if (filters[name]) {
                filters = Object.assign({}, filters);
                delete filters[name];
            }
            return {
                ...prevState,
                filters,
                page: 1,
                selectedRows: new Set(),
            };
        }, this.debounceSetFilterApiAndLoadList);
    };

    setSort = (e, { name, value }) => {
        const isDesc = value === 'desc';
        const sort =
            this.state.sort.name === name && this.state.sort.desc === isDesc
                ? {}
                : {
                      name: name,
                      desc: isDesc,
                  };
        const { storageSortItem } = this.props;

        storageSortItem && localStorage.setItem(storageSortItem, JSON.stringify(sort));

        this.setState(
            {
                sort,
                page: 1,
            },
            this.loadAndResetContainerScroll,
        );
    };

    setSelected = item => {
        this.setState({
            selectedRows: item,
        }, () => {this.props.getActions({name: this.props.name, ids: Array.from(item)})});
    };

    setSelectedAll = () => {
        const { selectedRows, filters: filter } = this.state;
        const { allIds = [], getAllIds, defaultFilter } = this.props;
        let newSelectedRows = new Set();

        if (selectedRows.size) {
            newSelectedRows = new Set();
            this.setSelected(newSelectedRows);
        } else if (allIds && allIds.length) {
            newSelectedRows = new Set(allIds);
            this.setSelected(newSelectedRows);
        } else {
            getAllIds({
                filter: { ...filter, ...defaultFilter },
                callbackSuccess: ids => {
                    newSelectedRows = new Set(ids);
                    this.setSelected(newSelectedRows);
                },
            });
        }
    };

    changeFullTextFilter = (e, { value }) => {
        this.setState({ fullText: value, page: 1 }, this.debounceSetFilterApiAndLoadList);
    };

    clearFilters = () => {
        const { storageFilterItem } = this.props;
        localStorage.setItem(storageFilterItem, JSON.stringify({}));
        this.setState(
            {
                filters: {},
                fullText: '',
                sort: {},
                page: 1,
                selectedRows: new Set(),
            },
            this.loadAndResetContainerScroll,
        );
    };

    setFilterApiAndLoadList = () => {
        const filtersJson = JSON.stringify(this.state.filters);
        const { storageFilterItem } = this.props;

        storageFilterItem && localStorage.setItem(storageFilterItem, filtersJson);
        this.loadAndResetContainerScroll();
    };

    debounceSetFilterApiAndLoadList = debounce(300, this.setFilterApiAndLoadList);

    loadAndResetContainerScroll = () => {
        this.loadList();
        this.container.scrollTop = 0;
    };

    getLookup = name => {
        const { getLookupList } = this.props;
        const { filters } = this.state;

        getLookupList({ filter: filters, field: name });
    };

    handleUnloadInExcel = () => {
        const { getReport } = this.props;
        const { filters: filter } = this.state;

        getReport({ filter });
    };

    render() {
        const { filters, sort, fullText, selectedRows, extSelectedRow } = this.state;
        const {
            totalCount: count = 0,
            columns,
            rows,
            progress,
            modalCard,
            catalogsFromGrid,
            actions,
            isShowActions,
            confirmation = {},
            closeConfirmation = () => {},
            stateColors,
            groupActions,
            createButton,
            extGrid,
            collapsed,
            onlyOneCheck,
            checkAllDisabled,
            disabledCheck,
            isUnloadInExcel,
            loadingReport,
            colorInfo,
            autoUpdateStop,
        } = this.props;

        console.log('ff', selectedRows, groupActions);

        return (
            <>
                <Dimmer active={progress} inverted className="table-loader">
                      <Loader size="huge">Loading</Loader> 
                </Dimmer>
                <HeaderSearchGrid
                    createButton={
                        createButton && (
                            <CreateButton
                                button={createButton}
                                loadList={this.loadList}
                                stopUpdate={autoUpdateStop}
                            />
                        )
                    }
                    searchValue={fullText}
                    searchOnChange={this.changeFullTextFilter}
                    counter={count}
                    disabledClearFilter={!Object.keys(filters).length && !fullText}
                    clearFilter={this.clearFilters}
                />
                <div
                    className={`scroll-grid-container${extGrid ? ' grid_small' : ''}`}
                    ref={instance => {
                        this.container = instance;
                    }}
                >
                    <InfiniteScrollTable
                        className="grid-table"
                        unstackable
                        celled={false}
                        selectable={false}
                        headerRow={
                            <Filter
                                columns={columns}
                                filters={filters}
                                indeterminate={!!(selectedRows.size && selectedRows.size !== count)}
                                all={!!(selectedRows.size && selectedRows.size === count)}
                                stateColors={stateColors}
                                catalogs={catalogsFromGrid}
                                isShowActions={isShowActions}
                                sort={sort}
                                checkAllDisabled={checkAllDisabled || onlyOneCheck}
                                getLookup={this.getLookup}
                                setFilter={this.setFilter}
                                setSort={this.setSort}
                                setSelectedAll={this.setSelectedAll}
                            />
                        }
                        context={this.container}
                        onBottomVisible={this.nextPage}
                    >
                        <Result
                            columns={columns}
                            rows={rows}
                            modalCard={modalCard}
                            actions={actions}
                            onlyOneCheck={onlyOneCheck}
                            stateColors={stateColors}
                            loadList={this.loadList}
                            disabledCheck={disabledCheck}
                            selectedRows={selectedRows}
                            setSelected={this.setSelected}
                            isShowActions={isShowActions}
                        />
                    </InfiniteScrollTable>
                </div>
                <Grid className="grid-footer-panel" columns="2">
                    <Grid.Row>
                        {isUnloadInExcel ? (
                            <Grid.Column>
                                <Button
                                    color="grey"
                                    content={'Экспортировать в Excel'}
                                    icon={'file excel'}
                                    loading={loadingReport}
                                    onClick={this.handleUnloadInExcel}
                                />
                            </Grid.Column>
                        ) : null}
                        <Grid.Column>
                            {selectedRows.size && groupActions
                                ? groupActions().map(action => (
                                      <span key={action.name}>
                                          <Button
                                              color={action.color}
                                              content={action.name}
                                              loading={action.loading}
                                              disabled={action.loading}
                                              icon={action.icon}
                                              size="mini"
                                              onClick={() =>
                                                  action.action(action.ids, () => {
                                                      this.setState(
                                                          {
                                                              selectedRows: new Set(),
                                                          },
                                                          () => this.loadList(false, true),
                                                      );
                                                  })
                                              }
                                          />
                                      </span>
                                  ))
                                : null}
                        </Grid.Column>
                        <Grid.Column floated="right">{colorInfo}</Grid.Column>
                    </Grid.Row>
                </Grid>
                <Confirm
                    dimmer="blurring"
                    open={confirmation.open}
                    onCancel={closeConfirmation}
                    onConfirm={confirmation.onConfirm}
                    content={confirmation.content}
                />
            </>
        );
    }
}

SuperGrid.propTypes = {
    totalCount: PropTypes.number,
    columns: PropTypes.array.isRequired,
    rows: PropTypes.array.isRequired,
    progress: PropTypes.bool,
    loadList: PropTypes.func,
    autoUpdateStart: PropTypes.func,
    autoUpdateStop: PropTypes.func,
};
SuperGrid.defaultProps = {
    loadList: () => {},
    autoUpdateStart: () => {},
    autoUpdateStop: () => {},
    confirmation: {},
    closeConfirmation: () => {},
    clearStore: () => {},
    getLookupList: () => {},
    getAllIds: () => {},
    disabledCheck: () => {},
};

export default SuperGrid;
