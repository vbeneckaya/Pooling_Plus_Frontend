import React, {Component} from 'react';
import {withTranslation} from 'react-i18next';
import {Button, Checkbox, Loader, Table} from 'semantic-ui-react';
import BodyCell from './body_cell';
import {connect} from 'react-redux';
import {checkForEditingRequest} from '../../../ducks/gridColumnEdit';
import {invokeMassUpdateRequest} from '../../../ducks/gridActions';
import _ from 'lodash';

class Result extends Component {
    shouldComponentUpdate(nextProps) {
        if (nextProps.rows && this.props.rows && nextProps.rows.length !== this.props.rows.length) {
            return true;
        }

        if (this.props.progress !== nextProps.progress) {
            return true;
        }

        if (!_.isEqual(Array.from(nextProps.selectedRows), Array.from(this.props.selectedRows))) {
            return true;
        }

        if (!_.isEqual(Array.from(nextProps.columns), Array.from(this.props.columns))) {
            return true;
        }

        if (_.isEqual(nextProps.rows, this.props.rows)) {
            return false;
        }

        return true;
    }

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
            modalCard,
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
                        className={`grid-row${ row.color || ''} ${selectedRows.has(row.id) ? 'grid-row-selected' : ''}`}
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
                                key={`cell_${row.id}_${column.name}_${indexRow}`}
                                row={row}
                                column={column}
                                value={row[column.name]}
                                indexRow={indexRow}
                                indexColumn={indexColumn}
                                loadList={loadList}
                                gridName={name}
                                modalCard={modalCard}
                                t={t}
                                checkForEditing={checkForEditing}
                                invokeMassUpdate={invokeMassUpdate}
                            />
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
