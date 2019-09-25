import React from 'react';
import { useTranslation } from 'react-i18next';
import { Checkbox, Form, Grid, Segment } from 'semantic-ui-react';
import Select from '../../BaseComponents/Select';
import State from '../../BaseComponents/State';
import Date from '../../BaseComponents/Date';

const Information = ({ form = {}, onChange }) => {
    const { t } = useTranslation();

    return (
        <Form>
            <Grid>
                <Grid.Row columns={5}>
                    <Grid.Column>
                        <State
                            name="shippingState"
                            value={form['shippingState']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Select
                            name="deliveryMethod"
                            value={form['deliveryMethod']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Select name="tariff" value={form['tariff']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Select
                            name="transportType"
                            value={form['transportType']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Select
                            name="thermalMode"
                            value={form['thermalMode']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('documents')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row>
                                        <Grid.Column>
                                            <Checkbox
                                                checked={form['waybill']}
                                                label={t('waybill')}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row>
                                        <Grid.Column>
                                            <Checkbox
                                                checked={form['waybillTorg12']}
                                                label={t('waybillTorg12')}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row>
                                        <Grid.Column>
                                            <Checkbox
                                                checked={form['waybillTransportSection']}
                                                label={t('waybillTransportSection')}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row>
                                        <Grid.Column>
                                            <Checkbox
                                                checked={form['invoice']}
                                                label={t('invoice')}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row>
                                        <Grid.Column width={10}>
                                            <Date
                                                name="actualReturnDate"
                                                value={form['actualReturnDate']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        </Form>
    );
};

export default Information;
