import React from 'react';
import { useTranslation } from 'react-i18next';
import { Checkbox, Form, Grid, Segment } from 'semantic-ui-react';
import Select from '../../BaseComponents/Select';
import State from '../../BaseComponents/State';
import Date from '../../BaseComponents/Date';
import Text from '../../BaseComponents/Text';
import TextArea from '../../BaseComponents/TextArea';
import Number from '../../BaseComponents/Number';

const Information = ({ form = {}, onChange }) => {
    const { t } = useTranslation();

    return (
        <Form>
            <Grid>
                {/* <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('general info')}</label>
                            <Segment>
                                <Grid>*/}
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <Select
                            name="carrierId"
                            value={form['carrierId']}
                            source="transportCompanies"
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Select
                            name="deliveryType"
                            value={form['deliveryType']}
                            isTranslate
                            source="deliveryType"
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Select
                            name="tarifficationType"
                            value={form['tarifficationType']}
                            isTranslate
                            source="tarifficationType"
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <Select
                            name="vehicleTypeId"
                            value={form['vehicleTypeId']}
                            source="vehicleTypes"
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('temperature')}</label>
                            <div className="temperature-fields">
                                <label>{t('from')}</label>
                                <Number
                                    noLabel
                                    name="temperatureMin"
                                    value={form['temperatureMin']}
                                    onChange={onChange}
                                />
                                <label>{t('to')}</label>
                                <Number
                                    noLabel
                                    name="temperatureMax"
                                    value={form['temperatureMax']}
                                    onChange={onChange}
                                />
                            </div>
                        </Form.Field>
                    </Grid.Column>
                    <Grid.Column>
                        <Number
                            name="totalDeliveryCost"
                            value={form['totalDeliveryCost']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                {/*  </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>*/}
                <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('palletsCountGroup')}</label>
                            <Segment className="mini-column">
                                <Grid>
                                    <Grid.Row columns={3}>
                                        <Grid.Column>
                                            <Number
                                                name="palletsCount"
                                                text="prepare"
                                                value={form['palletsCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Number
                                                name="confirmedPalletsCount"
                                                text="plan"
                                                value={form['confirmedPalletsCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Number
                                                name="actualPalletsCount"
                                                text="fact"
                                                value={form['actualPalletsCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('weigth')}</label>
                            <Segment className="mini-column">
                                <Grid>
                                    <Grid.Row columns={3}>
                                        <Grid.Column>
                                            <Number
                                                name="weightKg"
                                                text="planWeigth"
                                                value={form['weightKg']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Number
                                                name="actualWeightKg"
                                                text="factWeigth"
                                                value={form['actualWeightKg']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>
                {/* <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('documents')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={2} stretched>
                                        <Grid.Column>
                                            <Checkbox
                                                checked={form['waybill']}
                                                label={t('waybill')}
                                                onClick={event =>
                                                    onChange(event, {
                                                        name: 'waybill',
                                                        value: !form['waybill'],
                                                    })
                                                }
                                            />
                                            <Checkbox
                                                checked={form['waybillTorg12']}
                                                label={t('waybillTorg12')}
                                                onClick={event =>
                                                    onChange(event, {
                                                        name: 'waybillTorg12',
                                                        value: !form['waybillTorg12'],
                                                    })
                                                }
                                            />
                                            <Checkbox
                                                checked={form['transportWaybill']}
                                                label={t('transportWaybill')}
                                                onClick={event =>
                                                    onChange(event, {
                                                        name: 'transportWaybill',
                                                        value: !form['transportWaybill'],
                                                    })
                                                }
                                            />
                                            <Checkbox
                                                checked={form['invoice']}
                                                label={t('invoice')}
                                                onClick={event =>
                                                    onChange(event, {
                                                        name: 'invoice',
                                                        value: !form['invoice'],
                                                    })
                                                }
                                            />
                                        </Grid.Column>
                                        <Grid.Column className="mini-column">
                                            <Date
                                                name="documentsReturnDate"
                                                value={form['documentsReturnDate']}
                                                onChange={onChange}
                                            />
                                            <Date
                                                name="actualDocumentsReturnDate"
                                                value={form['actualDocumentsReturnDate']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>*/}
            </Grid>
        </Form>
    );
};

export default Information;
