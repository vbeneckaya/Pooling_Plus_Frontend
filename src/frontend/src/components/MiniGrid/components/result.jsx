import React, { Component } from 'react';
import { withTranslation } from 'react-i18next';
import { Button, Loader, Table } from 'semantic-ui-react';
import BodyCell from './body_cell';
import { connect } from 'react-redux';
import NotFoundMessage from './notFoundMessage';
import { checkForEditingRequest } from '../../../ducks/gridColumnEdit';
import {ORDERS_GRID} from "../../../constants/grids";
import {invokeMassUpdateRequest} from "../../../ducks/gridActions";

class Result extends Component {
    // handleCheck = row => {
    //     const { selectedRows, setSelected, onlyOneCheck } = this.props;
    //     let newSelectedRows;
    //     if (onlyOneCheck) {
    //         newSelectedRows = new Set();
    //         if (!selectedRows.has(row.id)) {
    //             newSelectedRows.add(row.id);
    //         }
    //         setSelected(newSelectedRows);
    //     } else {
    //         newSelectedRows = new Set(selectedRows);
    //         newSelectedRows[!selectedRows.has(row.id) ? 'add' : 'delete'](row.id);
    //         setSelected(newSelectedRows);
    //     }
    // };
    
    
    render() {
        const {
            columns = [],
            rows = [],
        //    goToCard,
          //  actions,
        //    isShowActions,
            removeFromShipping,
            isShowDeleteButton,
            name,
            progress,
            openOrderModal,
            t,
            checkForEditing,
            invokeMassUpdate,
            loadCard,
        } = this.props;
        
        const  handleDeleteClick = (id) => {
            !!removeFromShipping && removeFromShipping(id);
        }

         return (
            <Table.Body>
                {progress === null ? null : rows && rows.length ? (
                    rows.map((row, indexRow) => (
                        <Table.Row
                            key={row.id}
                            className={`grid-row`}
                            data-grid-id={row.id}
                        >
                            <Table.Cell
                            key={row.id + 'checkbox'}
                            className="small-column"
                            >
                                <span>{indexRow+1}</span>
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
                                    rowNumber={row.orderNumber}
                                    column={column}
                                    indexRow={indexRow}
                                    indexColumn={indexColumn}
                                    gridName={name}
                                    goToCard={null}
                                    loadCard={loadCard}
                                    openOrderModal={openOrderModal}
                                    t={t}
                                    checkForEditing={checkForEditing}
                                    invokeMassUpdate={invokeMassUpdate}
                                />
                            ))}
                            <Table.Cell/>
                            {isShowDeleteButton  ? (
                                <Table.HeaderCell
                                    className="actions-column"
                                    onClick={e => {
                                        e.stopPropagation();
                                    }}
                                >
                                   
                                        <Button icon="remove"
                                            key={row.id + 'delete_from_shipping'}
                                            actionname={'delete_from_shipping'}
                                            actionbuttonname={'delete_from_shipping'}
                                            rowid={row.id}
                                            // //disabled={action.disabled}
                                            className="grid-action-btn"
                                            // loading={
                                            //     action.loadingId &&
                                            //     action.loadingId.includes(row.id)
                                            // }
                                            onClick={()=>handleDeleteClick(row.id)}
                                            size="mini"
                                        />
                                 
                                </Table.HeaderCell>
                            ) : null}
                        </Table.Row>
                    ))
                ) : !progress ? (
                    <NotFoundMessage
                        gridName={ORDERS_GRID}
                       // goToCard={goToCard}
                    />
                ) : null}
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
    }
};

export default withTranslation()(
    connect(
        null,
        mapDispatchToProps,
    )(Result),
);
