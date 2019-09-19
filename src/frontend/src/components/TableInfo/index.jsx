import React, { Component } from 'react';
import { Button, Container, Dimmer, Grid, Loader, Table } from 'semantic-ui-react';
import InfiniteScrollTable from '../InfiniteScrollTable';
import { debounce } from 'throttle-debounce';
import { PAGE_SIZE } from '../../constants/settings';
import Search from '../Search';
import './style.scss';
import CellValue from '../ColumnsValue';
import { withTranslation } from 'react-i18next';

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
            console.log('!!!!', this.props.name, this.props.headerRow);
            this.load();
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
        this.setState({ filter: value, page: 1 }, this.debounceSetFilterApiAndLoadList);
    };

    debounceSetFilterApiAndLoadList = debounce(300, this.load);

    headerRowComponent = () => (
        <Table.Row>
            {this.props.headerRow &&
                this.props.headerRow.map(row => (
                    <Table.HeaderCell className="table-header-cell" key={row.name}>
                        {this.props.t(row.name)}
                    </Table.HeaderCell>
                ))}
            {this.props.isShowActions ? <Table.HeaderCell /> : null}
        </Table.Row>
    );

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
            modalCard
        } = this.props;

        const { filter } = this.state;

        return (
            <Container className={className}>
                <Dimmer active={loading} inverted className="table-loader">
                    <Loader size="huge">Loading</Loader>
                </Dimmer>
                <div className="table-header-menu">
                    <h2>{t(title)}</h2>
                    <Grid>
                        <Grid.Row>
                            <Grid.Column width={7}>
                                <Search
                                    isAuto
                                    value={filter}
                                    onChange={this.changeFullTextFilter}
                                />
                            </Grid.Column>
                            <Grid.Column width={9} textAlign="right">
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
                            </Grid.Column>
                        </Grid.Row>
                    </Grid>
                </div>
                <div
                    className={`scroll-grid-container`}
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
                                  list.map(row => (
                                      <ModalComponent
                                          element={modalCard}
                                          props={{ id: row.id, loadList: this.load, name }}
                                          key={`modal_${row.id}`}
                                      >
                                          <Table.Row key={row.id}>
                                              {headerRow.map((column, index) => (
                                                  <Table.Cell
                                                      key={`cell_${row.id}_${column.name}_${index}`}
                                                  >
                                                      <CellValue
                                                          type={column.type}
                                                          id={`${row.id}_${column.name}_${index}`}
                                                          toggleIsActive={toggleIsActive}
                                                          value={row[column.name]}
                                                      />
                                                  </Table.Cell>
                                              ))}
                                              {isShowActions ? (
                                                  <Table.Cell textAlign="center">
                                                      {actions &&
                                                          actions(row, this.load).map(
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
                        </Table.Body>
                    </InfiniteScrollTable>
                </div>
            </Container>
        );
    }
}

TableInfo.defaultProps = {
    loadList: () => {},
};

export default withTranslation()(TableInfo);
