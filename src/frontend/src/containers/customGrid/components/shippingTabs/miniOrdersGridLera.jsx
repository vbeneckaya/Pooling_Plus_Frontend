import React, {Component} from 'react';
import {useTranslation, withTranslation} from 'react-i18next';
import {Button, Form, Grid, Loader, Segment} from 'semantic-ui-react';
import Table from "semantic-ui-react/dist/commonjs/collections/Table/Table";
import BodyCell from "../../../../components/SuperGrid/components/body_cell";
import {ORDERS_GRID} from "../../../../constants/grids";
import NotFoundMessage from "../../../../components/SuperGrid/components/notFoundMessage";

class MiniOrdersGrid extends Component {
    render() {
        const {
            rows = [],
            columns=[],
            goToCard,
            actions,
            isShowActions,
            loadList,
            disabledCheck,
            name,
            progress,
            t,
            checkForEditing,
            invokeMassUpdate,
            isSetFilters,
            isCreateBtn,

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
                            {/*<Table.Cell*/}
                                {/*key={row.id + 'checkbox'}*/}
                                {/*className="small-column"*/}
                                {/*onClick={e => {*/}
                                    {/*e.stopPropagation();*/}
                                {/*}}*/}
                            {/*>*/}
                                {/*<div*/}
                                    {/*className={`${row.highlightForConfirmed ? 'grid-marker' : ''}`}*/}
                                {/*/>*/}
                                {/*<CustomCheckbox*/}
                                    {/*checked={!!selectedRows.has(row.id)}*/}
                                    {/*disabled={disabledCheck(row)}*/}
                                    {/*onChange={() => {*/}
                                        {/*this.handleCheck(row);*/}
                                    {/*}}*/}
                                {/*/>*/}
                            {/*</Table.Cell>*/}
                            
                            
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
                    ))
                ) : !progress ? (
                    <NotFoundMessage
                        gridName={name}
                        isSetFilters={isSetFilters}
                        isCreateBtn={isCreateBtn}
                        goToCard={goToCard}
                    />
                ) : null}
                <div className="table-bottom-loader">
                    <Loader active={progress && rows.length}/>
                </div>
            </Table.Body>
        );
    }
}

export default withTranslation()(MiniOrdersGrid);
   
    

