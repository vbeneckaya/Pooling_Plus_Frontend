import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withTranslation } from 'react-i18next';

import { debounce } from 'throttle-debounce';

import './style.scss';
import Filter from './components/filter';
import HeaderSearchGrid from './components/header';
import InfiniteScrollTable from '../InfiniteScrollTable';

import Result from './components/result';
import { PAGE_SIZE } from '../../constants/settings';
import { Confirm } from 'semantic-ui-react';
import Footer from './components/footer';

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
        this.props.autoUpdateStop({ isClear: true });
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
        const { autoUpdateStop, autoUpdateStart } = this.props;
        const { selectedRows } = this.state;

        autoUpdateStop();
        autoUpdateStart(this.mapData(isConcat, isReload));

        if (selectedRows.size) {
            this.props.getActions({ name: this.props.name, ids: Array.from(selectedRows) });
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
            if (value !== null && value !== undefined) filters = { ...filters, [name]: value };
            else if (filters[name] !== null && filters[name] !== undefined) {
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

    setSort = (sort) => {
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
        this.setState(
            {
                selectedRows: item,
            },
            () => {
                this.props.getActions({ name: this.props.name, ids: Array.from(item) });
            },
        );
    };

    setSelectedAll = () => {
        const { selectedRows, filters: filter } = this.state;
        const { allIds = [], getAllIds, defaultFilter, name } = this.props;
        let newSelectedRows = new Set();

        if (selectedRows.size) {
            newSelectedRows = new Set();
            this.setSelected(newSelectedRows);
        } else if (allIds && allIds.length) {
            newSelectedRows = new Set(allIds);
            this.setSelected(newSelectedRows);
        } else {
            getAllIds({
                name,
                filter: this.mapData().filter,
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

    clearSelectedRows = () => {
        this.setState(
            {
                selectedRows: new Set(),
            },
            () => this.loadList(false, true),
        );
    };

    updatingFilter = () => {
        const { filters } = this.state;
        const { columns, storageFilterItem } = this.props;

        let newFilter = {};

        Object.keys(filters).forEach(key => {
            if (columns.find(item => item.name === key)) {
                newFilter = {
                    ...newFilter,
                    [key]: filters[key],
                };
            }
        });
        this.setState(
            {
                filters: newFilter,
            },
            this.debounceSetFilterApiAndLoadList,
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

    render() {
        const { filters, sort, fullText, selectedRows } = this.state;
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
            groupActions,
            createButton,
            extGrid,
            onlyOneCheck,
            checkAllDisabled,
            disabledCheck,
            autoUpdateStop,
            storageRepresentationItems,
            name,
            t,
        } = this.props;

        return (
            <>
                <HeaderSearchGrid
                    createButton={
                        createButton && (
                            <CreateButton
                                button={createButton}
                                loadList={this.loadList}
                                stopUpdate={autoUpdateStop}
                                name={name}
                            />
                        )
                    }
                    name={name}
                    loadList={this.loadList}
                    searchValue={fullText}
                    searchOnChange={this.changeFullTextFilter}
                    counter={count}
                    storageRepresentationItems={storageRepresentationItems}
                    disabledClearFilter={!Object.keys(filters).length && !fullText}
                    clearFilter={this.clearFilters}
                    updatingFilter={this.updatingFilter}
                    filter={filters}
                    setSelected={this.setSelected}
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
                                catalogs={catalogsFromGrid}
                                isShowActions={isShowActions}
                                sort={sort}
                                checkAllDisabled={checkAllDisabled || onlyOneCheck}
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
                            progress={progress}
                            name={name}
                            modalCard={modalCard}
                            actions={actions}
                            onlyOneCheck={onlyOneCheck}
                            loadList={this.loadList}
                            disabledCheck={disabledCheck}
                            selectedRows={selectedRows}
                            setSelected={this.setSelected}
                            isShowActions={isShowActions}
                        />
                    </InfiniteScrollTable>
                    {selectedRows.size ? (
                        <Footer
                            gridName={name}
                            groupActions={groupActions}
                            clearSelectedRows={this.clearSelectedRows}
                        />
                    ) : null}
                </div>
                <Confirm
                    dimmer="blurring"
                    open={confirmation.open}
                    onCancel={closeConfirmation}
                    onConfirm={confirmation.onConfirm}
                    cancelButton={t('cancelConfirm')}
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

export default withTranslation()(SuperGrid);
