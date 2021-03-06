import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid, Segment } from 'semantic-ui-react';
import FormField from '../../../../components/BaseComponents';
import { NUMBER_TYPE, SELECT_TYPE, TEXT_TYPE } from '../../../../constants/columnTypes';

const Information = ({ form = {}, onChange, settings }) => {
    const { t } = useTranslation();

    return (
        <Form className="tabs-card">
            <Grid>
                {/* <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('general info')}</label>
                            <Segment>
                                <Grid>*/}
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <FormField
                            name="carrierId"
                            value={form['carrierId']}
                            type={SELECT_TYPE}
                            settings={settings['carrierId']}
                            source="transportCompanies"
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="tarifficationType"
                            value={form['tarifficationType']}
                            isTranslate
                            type={SELECT_TYPE}
                            settings={settings['tarifficationType']}
                            source="tarifficationType"
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <FormField
                            name="vehicleTypeId"
                            value={form['vehicleTypeId']}
                            source="vehicleTypes"
                            type={SELECT_TYPE}
                            settings={settings['vehicleTypeId']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('temperature')}</label>
                            <div className="temperature-fields">
                                <label>{t('from')}</label>
                                <FormField
                                    noLabel
                                    name="temperatureMin"
                                    value={form['temperatureMin']}
                                    type={NUMBER_TYPE}
                                    settings={settings['temperatureMin']}
                                    onChange={onChange}
                                />
                                <label>{t('to')}</label>
                                <FormField
                                    noLabel
                                    name="temperatureMax"
                                    value={form['temperatureMax']}
                                    type={NUMBER_TYPE}
                                    settings={settings['temperatureMax']}
                                    onChange={onChange}
                                />
                            </div>
                        </Form.Field>
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="totalDeliveryCost"
                            value={form['totalDeliveryCost']}
                            type={NUMBER_TYPE}
                            settings={settings['totalDeliveryCost']}
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
                                            <FormField
                                                name="palletsCount"
                                                text="plan"
                                                value={form['palletsCount']}
                                                type={NUMBER_TYPE}
                                                settings={settings['palletsCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="confirmedPalletsCount"
                                                text="fact"
                                                value={form['confirmedPalletsCount']}
                                                type={NUMBER_TYPE}
                                                settings={settings['confirmedPalletsCount']}
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
                            <label>{t('weight')}</label>
                            <Segment className="mini-column">
                                <Grid>
                                    <Grid.Row columns={3}>
                                        <Grid.Column>
                                            <FormField
                                                name="weightKg"
                                                text="planWeight"
                                                value={form['weightKg']}
                                                type={NUMBER_TYPE}
                                                settings={settings['weightKg']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="confirmedWeightKg"
                                                text="factWeight"
                                                value={form['confirmedWeightKg']}
                                                type={NUMBER_TYPE}
                                                settings={settings['confirmedWeightKg']}
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
                            <label>{t('driverAndVehicleNumber')}</label>
                            <Segment className="mini-column">
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <FormField
                                                name="driver"
                                                text="driver"
                                                value={form['driver']}
                                                type={TEXT_TYPE}
                                                settings={settings['driver']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="vehicleNumber"
                                                text="vehicleNumber"
                                                value={form['vehicleNumber']}
                                                type={TEXT_TYPE}
                                                settings={settings['vehicleNumber']}
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
                            <label>{t('Pooling.Info')}</label>
                            <Segment className="mini-column">
                                <Grid>
                                    <Grid.Row columns={1}>
                                        <Grid.Column>
                                            <FormField
                                                name="Pooling.Info"
                                                text="Pooling.Info"
                                                value={form['poolingInfo']}
                                                type={TEXT_TYPE}
                                                settings={settings['poolingInfo']}
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
