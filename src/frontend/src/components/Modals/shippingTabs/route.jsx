import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid, Tab } from 'semantic-ui-react';
import Select from '../../BaseComponents/Select';
import TextArea from '../../BaseComponents/TextArea';
import DateTime from '../../BaseComponents/DateTime';

const Route = ({ name, form = {}, onChange }) => {
    const { t } = useTranslation();
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
            render: () => <Tab.Pane className="ext-tabs-card">Orders</Tab.Pane>,
        },
    ];

    return <Tab panes={panes} />;
};

export default Route;
