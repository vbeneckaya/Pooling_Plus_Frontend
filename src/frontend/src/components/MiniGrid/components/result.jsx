import React, { Component } from 'react';
import { withTranslation } from 'react-i18next';
import { Button, Loader, Table } from 'semantic-ui-react';
import BodyCell from './body_cell';
import { connect } from 'react-redux';
import { checkForEditingRequest } from '../../../ducks/gridColumnEdit';
import { invokeMassUpdateRequest } from '../../../ducks/gridActions';
import { ORDERS_GRID } from '../../../constants/grids';
import CustomCheckbox from '../../BaseComponents/CustomCheckbox';
import NotFoundMessage from './notFoundMessage';

class Result extends Component {
    render() {
        const {
            columns = [],
            rows = [],
        //    goToCard,
          //  actions,
        //    isShowActions,
            name,
            progress,
            openOrderModal,
            t,
        //    checkForEditing,
        } = this.props;

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
                            {/*{isShowActions ? (*/}
                                {/*<Table.HeaderCell*/}
                                    {/*className="actions-column"*/}
                                    {/*onClick={e => {*/}
                                        {/*e.stopPropagation();*/}
                                    {/*}}*/}
                                {/*>*/}
                                    {/*{actions &&*/}
                                    {/*actions(row).map(action => (*/}
                                        {/*<Button*/}
                                            {/*key={row.id + action.name}*/}
                                            {/*actionname={action.name}*/}
                                            {/*actionbuttonname={action.buttonName}*/}
                                            {/*rowid={row.id}*/}
                                            {/*disabled={action.disabled}*/}
                                            {/*className="grid-action-btn"*/}
                                            {/*loading={*/}
                                                {/*action.loadingId &&*/}
                                                {/*action.loadingId.includes(row.id)*/}
                                            {/*}*/}
                                            {/*onClick={e =>*/}
                                                {/*action.action(e, {*/}
                                                    {/*action,*/}
                                                    {/*row,*/}
                                                    {/*loadList,*/}
                                                {/*})*/}
                                            {/*}*/}
                                            {/*size="mini"*/}
                                        {/*>*/}
                                            {/*{action.buttonName}*/}
                                        {/*</Button>*/}
                                    {/*))}*/}
                                {/*</Table.HeaderCell>*/}
                            {/*) : null}*/}
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
