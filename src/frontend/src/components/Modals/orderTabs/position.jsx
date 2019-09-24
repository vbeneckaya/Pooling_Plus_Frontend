import React from 'react';
import { useTranslation } from 'react-i18next';
import { Table } from 'semantic-ui-react';
import TableInfo from '../../TableInfo';

const columns = [
    {
        name: 'nart',
    },
    {
        name: 'description',
    },
    {
        name: 'country',
    },
    {
        name: 'spgr',
    },
    {
        name: 'ean',
    },
    {
        name: 'expirationDate',
    },
    {
        name: 'weight',
    },
    {
        name: 'netWeight',
    },
    {
        name: 'quantity',
    },
    {
        name: 'return',
    },
];

const Position = ({ rows = [] }) => {
    const { t } = useTranslation();
    return (
        <Table className="wider container-margin-top-bottom">
            <Table.Header>
                <Table.Row>
                    {columns.map(column => (
                        <Table.HeaderCell key={column.name}>{t(column.name)}</Table.HeaderCell>
                    ))}
                </Table.Row>
            </Table.Header>
            <Table.Body>
                {rows.map((row, index) => (
                    <Table.Row key={row.id}>
                        {columns.map(column => (
                            <Table.Cell key={`cell_${row.id}_${column.name}_${index}`}>
                                  {row[column.name]} 
                            </Table.Cell>
                        ))}
                    </Table.Row>
                ))}
            </Table.Body>
        </Table>
    );
};

export default Position;
