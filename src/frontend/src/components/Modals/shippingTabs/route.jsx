import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid, Tab, Table } from 'semantic-ui-react';
import Select from '../../BaseComponents/Select';
import TextArea from '../../BaseComponents/TextArea';
import DateTime from '../../BaseComponents/DateTime';

const columns = [
    {
        name: 'number'
    },
    {
        name: 'status'
    },
    {
        name: 'operationType'
    }
];

const Route = ({ name, form = {}, onChange }) => {
    const { t } = useTranslation();
    const rows = [];
    const panes = [
        {
            menuItem: t('information'),
            render: () => (
                <Tab.Pane className="ext-tabs-card">
                    <Form>
                        <Grid>
                            <Grid.Row columns={3}>
                                <Grid.Column>
                                    <Select
                                        name="status"
                                        value={form['status']}
                                        onChange={onChange}
                                    />
                                </Grid.Column>
                                <Grid.Column>
                                    <DateTime
                                        name="status"
                                        value={form['status']}
                                        onChange={onChange}
                                    />
                                </Grid.Column>
                                <Grid.Column>
                                    <DateTime
                                        name="status"
                                        value={form['status']}
                                        onChange={onChange}
                                    />
                                </Grid.Column>
                            </Grid.Row>
                            <Grid.Row>
                                <Grid.Column>
                                    <TextArea
                                        name="address"
                                        value={form['address']}
                                        onChange={onChange}
                                    />
                                </Grid.Column>
                            </Grid.Row>
                        </Grid>
                    </Form>
                </Tab.Pane>
            ),
        },
        {
            menuItem: t('ordersTab', { count: 4 }),
            render: () => <Tab.Pane className="ext-tabs-card">
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
            </Tab.Pane>,
        },
    ];

    return <Tab panes={panes} />;
};

export default Route;
