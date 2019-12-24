import React, {Component} from 'react';
import {withTranslation} from 'react-i18next';
import {withRouter} from 'react-router-dom';
import {Button, Checkbox, Loader, Table} from 'semantic-ui-react';
import BodyCell from './body_cell';
import {connect} from 'react-redux';
import {checkForEditingRequest} from '../../../ducks/gridColumnEdit';
import {invokeMassUpdateRequest} from '../../../ducks/gridActions';
import _ from 'lodash';
import CellValue from '../../ColumnsValue';
import {ORDERS_GRID} from '../../../constants/grids';

class Result extends Component {
    handleCheck = row => {
        const {selectedRows, setSelected, onlyOneCheck} = this.props;
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
            goToCard,
            actions,
            isShowActions,
            selectedRows,
            loadList,
            disabledCheck,
            name,
            progress,
            t,
            checkForEditing,
            invokeMassUpdate,
        } = this.props;

        return (
            <Table.Body>
                {rows &&
                rows.map((row, indexRow) => (
                    <Table.Row
                        key={row.id}
                        className={`grid-row${row.color || ''} ${
                            selectedRows.has(row.id) ? 'grid-row-selected' : ''
                            }`}
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
                        columns.map((column, indexColumn) => (
                            <BodyCell
                                key={`cell_${row.id}_${column.name}`}
                                value={
                                    row[column.name] && typeof row[column.name] === 'object'
                                        ? row[column.name].value
                                        : row[column.name]
                                }
                                valueText={
                                    row[column.name] && typeof row[column.name] === 'object'
                                        ? row[column.name].name
                                        : null
                                }
                                status={row.status}
                                rowId={row.id}
                                rowNumber={
                                    name === ORDERS_GRID
                                        ? row.orderNumber
                                        : row.shippingNumber
                                }
                                column={column}
                                indexRow={indexRow}
                                indexColumn={indexColumn}
                                loadList={loadList}
                                gridName={name}
                                goToCard={goToCard}
                                t={t}
                                checkForEditing={checkForEditing}
                                invokeMassUpdate={invokeMassUpdate}
                            />
                        ))}
                        <Table.Cell/>
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
                    <Loader active={progress && rows.length}/>
                </div>
            </Table.Body>
        );
    }
}

const mapDispatchToProps = dispatch => {
    return {
        checkForEditing: params => {
            dispatch(checkForEditingRequest(params));
        },
        invokeMassUpdate: params => {
            dispatch(invokeMassUpdateRequest(params));
        },
    };
};

export default withTranslation()(
    connect(
        null,
        mapDispatchToProps,
    )(Result),
);
