import React from 'react';
import {Label, Table} from "semantic-ui-react";
import _ from 'lodash';

const TableHeader = ({ statusList, t }) => {
    return (
        <>
            <Table.Row>
                <Table.HeaderCell rowSpan="2">{t('field')}</Table.HeaderCell>
                <Table.HeaderCell rowSpan="2">{t('hidden')}</Table.HeaderCell>
                <Table.HeaderCell colSpan={statusList.length} textAlign="center">
                    {t('status')}
                </Table.HeaderCell>
            </Table.Row>
            <Table.Row className="ext-header-row">
                {statusList.map(status => (
                    <Table.HeaderCell key={status.name} textAlign="center">
                        <Label className="status-label-bottom" color={status.color}>
                            {t(status.name)}
                        </Label>
                    </Table.HeaderCell>
                ))}
            </Table.Row>
        </>
    )
};

export default React.memo(TableHeader, (prevProps, nextProps) => {
    return _.isEqual(prevProps.statusList, nextProps.statusList);
});
