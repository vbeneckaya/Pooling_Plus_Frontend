import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withTranslation } from 'react-i18next';

import { debounce } from 'throttle-debounce';
import _ from 'lodash';

import './style.scss';
import Filter from './components/filter';
import HeaderSearchGrid from './components/header';
import InfiniteScrollTable from '../InfiniteScrollTable';

import Result from './components/result';
import { PAGE_SIZE } from '../../constants/settings';
import { Confirm, Loader } from 'semantic-ui-react';
import Footer from './components/footer';
import { withRouter } from 'react-router-dom';

const initState = () => ({
    page: 1,
    fullText: '',
    selectedRows: new Set(),
    columns: [],
});

class SelectGrid extends Component {
    constructor(props) {
        super(props);

        this.state = {
            ...initState(),
        };
    }

    pageLoading = () => {
        const {getRepresentations, name} = this.props;

        getRepresentations({
            key: name,
            callBackFunc: columns => {
                const width = this.container.offsetWidth - 65;

                this.setState(
                    {
                        columns: columns.map(item => ({
                            ...item,
                            width: item.width || parseInt(width / columns.length),
                        })),
                    },
                    () => this.props.autoUpdateStart(this.mapData()),
                );
            },
        });
    };

    componentDidMount() {
        this.timer = null;
        const {location, history, getRepresentations, name} = this.props;
        const { state } = location;

        if (state) {
            this.setState(
                {
                    fullText: state.filter.search,
                },
                () => {
                    history.replace(location.pathname, null);
                    this.pageLoading();
                },
            );
        } else {
            this.pageLoading();
        }
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

        if (!_.isEqual(prevProps.columns, this.props.columns)) {
            const { columns } = this.props;
            const width = this.container.offsetWidth - 65;

            this.setState(
                {
                    page: 1,
                    columns: columns.map(item => ({
                        ...item,
                        width: item.width || parseInt(width / columns.length),
                    })),
                },
                this.loadList,
            );
        }
    }

    mapData = (isConcat, isReload) => {
        const {columns, page, fullText} = this.state;
        const { extParams, defaultFilter, name } = this.props;

        let filters = {};
        let sort = {};

        columns.forEach(column => {
            filters = {
                ...filters,
                [column.name]: column.filter,
            };

            if (column.sort === true || column.sort === false) {
                sort = {
                    name: column.name,
                    desc: column.sort,
                };
            }
        });

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
            const nextColumns = [...prevState.columns];
            let index = nextColumns.findIndex(item => item.name === name);
            nextColumns[index] = {
                ...nextColumns[index],
                filter: value,
            };

            return {
                ...prevState,
                columns: [...nextColumns],
                page: 1,
                selectedRows: new Set(),
            };
        }, this.debounceSetFilterApiAndLoadList);
    };

    setSort = sort => {
        this.setState(prevState => {
            const nextColumns = prevState.columns.map(column => ({
                ...column,
                sort: null,
            }));
            if (sort) {
                let index = nextColumns.findIndex(item => item.name === sort.name);
                nextColumns[index] = {
                    ...nextColumns[index],
                    sort: sort.desc,
                };
            }

            return {
                ...prevState,
                columns: [...nextColumns],
                page: 1,
            };
        }, this.debounceSetFilterApiAndLoadList);
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
        const {selectedRows} = this.state;
        const {allIds = [], getAllIds, name} = this.props;
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
        this.setState({ fullText: value, page: 1 }, this.setFilterApiAndLoadList);
    };

    clearFilters = () => {
        this.setState(prevState => {
            const {columns} = prevState;

            return {
                ...prevState,
                columns: columns.map(item => ({
                    ...item,
                    filter: '',
                    sort: null,
                })),
                page: 1,
                selectedRows: new Set(),
            };
        }, this.setFilterApiAndLoadList);
    };

    clearSelectedRows = () => {
        this.setState(
            {
                selectedRows: new Set(),
            },
            () => this.loadList(false, true),
        );
    };

    setFilterApiAndLoadList = () => {
        const {editRepresentation, representationName, name} = this.props;
        const {columns} = this.state;

        editRepresentation({
            key: name,
            name: representationName,
            oldName: representationName,
            value: columns,
        });
        this.loadAndResetContainerScroll();
    };

    debounceSetFilterApiAndLoadList = debounce(300, this.setFilterApiAndLoadList);

    loadAndResetContainerScroll = () => {
        this.loadList();
        if (this.container && this.container.scrollTop) {
            this.container.scrollTop = 0;
        }
    };

    resizeColumn = (size, index) => {
        const { columns } = this.state;
        clearTimeout(this.timer);
        this.setState(prevState => {
            const nextColumns = [...prevState.columns];
            nextColumns[index] = {
                ...nextColumns[index],
                width: size.width,
            };
            return {
                columns: nextColumns,
            };
        });

        let sum = 0;

        columns.forEach(item => {
            sum = sum + item.width + columns.length + 50;
        });
        this.timer = setTimeout(() => {
            const { editRepresentation, representationName, name, getRepresentations } = this.props;

            if (representationName) {
                editRepresentation({
                    key: name,
                    name: representationName,
                    oldName: representationName,
                    value: columns,
                    callbackSuccess: () => {
                        //getRepresentations({key: name});
                    },
                });
            }
        }, 2000);
        
    };

    handleGoToCard = (isEdit, id, name) => {
        const { history, cardLink, newLink} = this.props;

        history.push({
            pathname: isEdit
                ? cardLink.replace(':name', name).replace(':id', id)
                : newLink.replace(':name', name),
            state: {
                ...this.mapData().filter,
                pathname: history.location.pathname,
            },
        });
    };

    render() {
        const {fullText, selectedRows, columns} = this.state;
        const {
            totalCount: count = 0,
            rows = [],
            progress,
            catalogsFromGrid,
            actions,
            isShowActions,
            confirmation = {},
            closeConfirmation = () => {},
            groupActions,
            isCreateBtn,
            extGrid,
            onlyOneCheck,
            checkAllDisabled,
            disabledCheck,
            storageRepresentationItems,
            name,
            representationName,
            t,
        } = this.props;

        return (
            <>
                <Loader active={progress && !rows.length} size="huge" className="table-loader">
                    Loading
                </Loader>
                <HeaderSearchGrid
                    isCreateBtn={isCreateBtn}
                    goToCard={this.handleGoToCard}
                    name={name}
                    loadList={this.loadList}
                    searchValue={fullText}
                    searchOnChange={this.changeFullTextFilter}
                    counter={count}
                    storageRepresentationItems={storageRepresentationItems}
                    disabledClearFilter={!columns.find(column => column.filter)}
                    representationName={representationName}
                    clearFilter={this.clearFilters}
                    filter={this.mapData()}
                    setSelected={this.setSelected}
                />
                <div
                    className={`scroll-grid-container${extGrid ? ' grid_small' : ''}`}
                    ref={instance => {this.container = instance;}}
                >
                    <InfiniteScrollTable
                        className="grid-table"
                        unstackable
                        celled={false}
                        selectable={false}
                        columns={columns}
                        fixed
                        headerRow={
                            <Filter
                                columns={columns}
                                indeterminate={!!(selectedRows.size && selectedRows.size !== count)}
                                all={!!(selectedRows.size && selectedRows.size === count)}
                                catalogs={catalogsFromGrid}
                                isShowActions={isShowActions}
                                gridName={name}
                                checkAllDisabled={checkAllDisabled || onlyOneCheck}
                                setFilter={this.setFilter}
                                setSort={this.setSort}
                                setSelectedAll={this.setSelectedAll}
                                resizeColumn={this.resizeColumn}
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
                            goToCard={this.handleGoToCard}
                            actions={actions}
                            onlyOneCheck={onlyOneCheck}
                            loadList={this.loadList}
                            disabledCheck={disabledCheck}
                            selectedRows={selectedRows}
                            setSelected={this.setSelected}
                            isShowActions={isShowActions}
                            isSetFilters={columns.find(column => column.filter)}
                            isCreateBtn={isCreateBtn}
                        />
                    </InfiniteScrollTable>
                    {selectedRows.size ? (
                        <Footer
                            gridName={name}
                            groupActions={groupActions}
                            selectedRows={selectedRows}
                            clearSelectedRows={this.clearSelectedRows}
                            load={this.loadList}
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

SelectGrid.propTypes = {
    totalCount: PropTypes.number,
    columns: PropTypes.array.isRequired,
    rows: PropTypes.array.isRequired,
    progress: PropTypes.bool,
    loadList: PropTypes.func,
    autoUpdateStart: PropTypes.func,
    autoUpdateStop: PropTypes.func,
};
SelectGrid.defaultProps = {
    loadList: () => {},
    autoUpdateStart: () => {},
    autoUpdateStop: () => {},
    confirmation: {},
    closeConfirmation: () => {},
    clearStore: () => {},
    getLookupList: () => {},
    getAllIds: () => {},
    disabledCheck: () => {},
}
;

export default withTranslation()(withRouter(SelectGrid));
