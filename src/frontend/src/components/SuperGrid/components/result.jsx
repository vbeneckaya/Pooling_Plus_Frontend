import React, { Component } from 'react';
import { withTranslation } from 'react-i18next';
import { Button, Checkbox, Dimmer, Loader, Table } from 'semantic-ui-react';
import CellValue from '../../../components/ColumnsValue';
import InfiniteScrollTable from '../../InfiniteScrollTable';

const ModalComponent = ({ element, props, children }) => {
    if (!element) {
        return <>{children}</>;
    }
    return React.cloneElement(element, props, children);
};

class Result extends Component {
    handleCheck = row => {
        const { selectedRows, setSelected, onlyOneCheck } = this.props;
        let newSelectedRows;
        if (onlyOneCheck) {
            newSelectedRows = new Set();
            if (!selectedRows.has(row.id)) {
                newSelectedRows.add(row.id);
            }
            setSelected(newSelectedRows);
        } else {
            newSelectedRows = new Set(selectedRows);
            newSelectedRows[!selectedRows.has(row.id) ? 'add' : 'delete'](row.id);
            setSelected(newSelectedRows);
        }
    };

    render() {
        const {
            columns = [],
            rows = [],
            modalCard,
            actions,
            isShowActions,
            selectedRows,
            loadList,
            disabledCheck,
            name,
            t,
            progress,
        } = this.props;

        return (
            <Table.Body>
                <Dimmer
                    active={progress && !rows.length}
                    inverted
                    className="table-loader table-loader-big"
                >
                    <Loader size="huge">Loading</Loader>
                </Dimmer>
                {rows &&
                    rows.map((row, i) => (
                        <Table.Row
                            key={row.id}
                            className={'grid-row ' + row.color || ''}
                            data-grid-id={row.id}
                        >
                            <Table.Cell
                                key={row.id + 'checkbox'}
                                className="small-column"
                                onClick={e => {
                                    e.stopPropagation();
                                }}
                            >
                                <Checkbox
                                    checked={!!selectedRows.has(row.id)}
                                    disabled={disabledCheck(row)}
                                    onChange={() => {
                                        this.handleCheck(row);
                                    }}
                                />
                            </Table.Cell>
                            {columns &&
                                columns.map(column => (
                                    <Table.Cell
                                        key={`cell_${row.id}_${column.name}_${i}`}
                                        className={
                                            column.name.toLowerCase().includes('address')
                                                ? 'address-cell'
                                                : ''
                                        }
                                    >
                                        {
                                            <CellValue
                                                {...column}
                                                id={`${row.id}_${column.name}_${i}`}
                                                indexRow={i}
                                                value={row[column.name]}
                                                modalCard={
                                                    <ModalComponent
                                                        element={modalCard}
                                                        props={{
                                                            ...row,
                                                            loadList,
                                                            title: `edit_${name}`,
                                                        }}
                                                        key={`modal_${row.id}`}
                                                    />
                                                }
                                            />
                                        }
                                    </Table.Cell>
                                ))}
                            {isShowActions ? (
                                <Table.HeaderCell
                                    className="actions-column"
                                    onClick={e => {
                                        e.stopPropagation();
                                    }}
                                >
                                    {actions &&
                                        actions(row).map(action => (
                                            <Button
                                                key={row.id + action.name}
                                                actionname={action.name}
                                                actionbuttonname={action.buttonName}
                                                rowid={row.id}
                                                disabled={action.disabled}
                                                className="grid-action-btn"
                                                loading={
                                                    action.loadingId &&
                                                    action.loadingId.includes(row.id)
                                                }
                                                onClick={e =>
                                                    action.action(e, {
                                                        action,
                                                        row,
                                                        loadList,
                                                    })
                                                }
                                                size="mini"
                                            >
                                                {action.buttonName}
                                            </Button>
                                        ))}
                                </Table.HeaderCell>
                            ) : null}
                        </Table.Row>
                    ))}
                <div className="table-bottom-loader">
                    <Loader active={progress && rows.length} />
                </div>
            </Table.Body>
        );
    }
}

export default withTranslation()(Result);
