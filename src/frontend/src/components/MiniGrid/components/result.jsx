import React, { Component } from 'react';
import { withTranslation } from 'react-i18next';
import { Button, Loader, Table } from 'semantic-ui-react';
import BodyCell from './body_cell';
import { connect } from 'react-redux';
import NotFoundMessage from './notFoundMessage';

class Result extends Component {
    // constructor(props){
    //     super(props);
    //     this.removeFromShipping= this.props.removeFromShipping;
    //     this.handleDeleteClick = this.handleDeleteClick.bind(this);
    // }

    // handleDeleteClick(indexRow) {
    //     !!this.removeFromShipping && this.removeFromShipping(indexRow);
    // }
    
    render() {
        const {
            columns = [],
            rows = [],
        //    goToCard,
          //  actions,
        //    isShowActions,
            isShowEditButton,
            removeFromShipping,
            isShowDeleteButton,
            name,
            progress,
            openOrderModal,
            t,
        //    checkForEditing,
        } = this.props;
        
        const  handleDeleteClick = (indexRow) => {
            !!removeFromShipping && removeFromShipping(indexRow);
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
                                    openOrderModal={openOrderModal}
                                    t={t}
                                    checkForEditing={()=>{}}
                                />
                            ))}
                            <Table.Cell/>
                            {isShowEditButton || isShowDeleteButton  ? (
                                <Table.HeaderCell
                                    className="actions-column"
                                    // onClick={e => {
                                    //     e.stopPropagation();
                                    // }}
                                    //onClick={handleDeleteClick(indexRow)}
                                    //onClick={removeFromShipping(indexRow)}
                                >
                                   
                                        <Button
                                            key={row.id + 'delete_from_shipping'}
                                            // actionname={'delete_from_shipping'}
                                            // actionbuttonname={'delete_from_shipping'}
                                            rowid={row.id}
                                            // //disabled={action.disabled}
                                            // className="grid-action-btn"
                                            // // loading={
                                            // //     action.loadingId &&
                                            // //     action.loadingId.includes(row.id)
                                            // // }
                                            onClick={()=>handleDeleteClick(indexRow)}
                                            size="mini"
                                        >
                                            {'remove'}
                                        </Button>
                                 
                                </Table.HeaderCell>
                            ) : null}
                        </Table.Row>
                    ))
                ) : !progress ? (
                    <NotFoundMessage
                        gridName={name}
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

// const mapDispatchToProps = dispatch => {
//     return {
//         checkForEditing: params => {
//             dispatch(checkForEditingRequest(params));
//         },
// };

export default withTranslation()(
    connect(
        null,
        null,
    )(Result),
);
