import React from 'react';
import { useTranslation } from 'react-i18next';
import { Button, Form, Grid, Table } from 'semantic-ui-react';
import TableInfo from '../../TableInfo';
import Text from '../../BaseComponents/Text';

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

const Position = ({ form, onChange }) => {
    const { t } = useTranslation();
    const { items = [] } = form;
    return (
        <>
            <Grid>
                <Grid.Row columns="equal">
                    <Grid.Column width={4}>
                        <Form>
                            <Text
                                name="orderAmountExcludingVAT"
                                value={form['orderAmountExcludingVAT']}
                                onChange={onChange}
                            />
                        </Form>
                    </Grid.Column>
                    <Grid.Column className="add-right-elements">
                        <Button>{t('addButton')}</Button>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row>
                    <Table className="wider container-margin-top-bottom">
                        <Table.Header>
                            <Table.Row>
                                {columns.map(column => (
                                    <Table.HeaderCell key={column.name}>
                                        {t(column.name)}
                                    </Table.HeaderCell>
                                ))}
                            </Table.Row>
                        </Table.Header>
                        <Table.Body>
                            {items.map((row, index) => (
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
                </Grid.Row>
            </Grid>
        </>
    );
};

export default Position;
