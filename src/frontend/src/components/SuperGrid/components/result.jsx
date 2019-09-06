import React, { Component } from 'react';
import { Button, Checkbox, Table } from 'semantic-ui-react';
import CellValue from './cell_value';

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
            stateColors,
            loadList,
            disabledCheck,
            isFixing
        } = this.props;

        const columnStyle = column => ({
            maxWidth: column.width + 'px',
            minWidth: column.width + 'px',
        });

        return (
            <Table.Body>
                {rows &&
                    rows.map((row, i) => (
                        <ModalComponent
                            element={modalCard}
                            props={{ ...row, loadList, isFixing }}
                            key={`modal_${row.id}`}
                        >
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
                                            key={`cell_${row.id}_${column.key}_${i}`}
                                            style={columnStyle(column)}
                                        >
                                            {
                                                <CellValue
                                                    type={column.type}
                                                    id={`${row.id}_${column.key}_${i}`}
                                                    value={row[column.key]}
                                                    stateColors={stateColors}
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
                        </ModalComponent>
                    ))}
            </Table.Body>
        );
    }
}

export default Result;
