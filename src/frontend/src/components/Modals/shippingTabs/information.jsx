import React from 'react';
import { useTranslation } from 'react-i18next';
import { Checkbox, Form, Grid, Segment } from 'semantic-ui-react';
import Select from '../../BaseComponents/Select';
import State from '../../BaseComponents/State';
import Date from '../../BaseComponents/Date';
import Text from "../../BaseComponents/Text";
import TextArea from "../../BaseComponents/TextArea";

const Information = ({ form = {}, onChange }) => {
    const { t } = useTranslation();

    return (
        <Form>
            <Grid>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <Select
                            name="transportCompany"
                            value={form['transportCompany']}
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
                        <Select name="billingMethod" value={form['billingMethod']} onChange={onChange} />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <Select
                            name="transportType"
                            text="Тип ТС"
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
                    <Grid.Column>
                        <Text
                            name="thermalMode"
                            text="Общая стоимость перевозки"
                            value={form['thermalMode']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('palletsCountGroup')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={3}>
                                        <Grid.Column>
                                            <Text
                                                name="palletsCount"
                                                text="prepare"
                                                value={form['palletsCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Text
                                                name="confirmedPalletsCount"
                                                text="plan"
                                                value={form['confirmedPalletsCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Text
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
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <Text
                                                name="weightKg"
                                                text="planWeigth"
                                                value={form['weightKg']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Text
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
                <Grid.Row>
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
                                                onChange={onChange}
                                            />
                                            <Checkbox
                                                checked={form['waybillTorg12']}
                                                label={t('waybillTorg12')}
                                                onChange={onChange}
                                            />
                                            <Checkbox
                                                checked={form['waybillTransportSection']}
                                                label={t('waybillTransportSection')}
                                                onChange={onChange}
                                            />
                                            <Checkbox
                                                checked={form['invoice']}
                                                label={t('invoice')}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column className="mini-column">
                                            <Date
                                                name="plannedReturnDate"
                                                value={form['plannedReturnDate']}
                                                onChange={onChange}
                                            />
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
