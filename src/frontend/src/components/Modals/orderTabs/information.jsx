import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid, Segment } from 'semantic-ui-react';
import Text from '../../BaseComponents/Text';
import State from '../../BaseComponents/State';
import Date from '../../BaseComponents/Date';
import Select from '../../BaseComponents/Select';
import TextArea from '../../BaseComponents/TextArea';

const Information = ({ form, onChange }) => {

    const { t } = useTranslation();

    return (
        <Form>
            <Grid>
                <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('general info')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={4}>
                                        <Grid.Column>
                                            <Text
                                                name="orderNumber"
                                                value={form['orderNumber']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Text
                                                name="payer"
                                                value={form['payer']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Text
                                                name="clientName"
                                                value={form['clientName']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Select
                                                name="pickingTypeId"
                                                value={form['pickingTypeId']}
                                                source="pickingTypes"
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row columns={4}>
                                        <Grid.Column>
                                            <Date
                                                name="orderDate"
                                                value={form['orderDate']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Select
                                                name="orderType"
                                                value={form['orderType']}
                                                source="orderType"
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Select
                                                name="soldTo"
                                                value={form['soldTo']}
                                                source="soldTo"
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Form.Field>
                                                <label>{t('temperature')}</label>
                                                <div className="temperature-fields">
                                                    <label>{t('from')}</label>
                                                    <Text
                                                        noLabel
                                                        name="temperatureMin"
                                                        value={form['temperatureMin']}
                                                        onChange={onChange}
                                                    />
                                                    <label>{t('to')}</label>
                                                    <Text
                                                        noLabel
                                                        name="temperatureMax"
                                                        value={form['temperatureMax']}
                                                        onChange={onChange}
                                                    />
                                                </div>
                                            </Form.Field>
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
                            <label>{t('route')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <TextArea
                                                name="shippingAddress"
                                                text="addressFrom"
                                                value={form['shippingAddress']}
                                                rows={2}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <TextArea
                                                name="deliveryAddress"
                                                text="addressTo"
                                                value={form['deliveryAddress']}
                                                rows={2}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row columns={2}>
                                        <Grid.Column className="mini-column">
                                            <Date
                                                name="shippingDate"
                                                value={form['shippingDate']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column className="mini-column">
                                            <Date
                                                name="deliveryDate"
                                                value={form['deliveryDate']}
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
                            <label>{t('palletsCountGroup')}</label>
                            <Segment className="mini-column">
                                <Grid>
                                    <Grid.Row columns={4}>
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
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('boxesCountGroup')}</label>
                            <Segment className="mini-column">
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <Text
                                                name="boxesCount"
                                                text="prepare"
                                                value={form['boxesCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Text
                                                name="confirmedBoxesCount"
                                                text="fact"
                                                value={form['confirmedBoxesCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('weigth')}</label>
                            <Segment
                                style={{ height: 'calc(100% - 22px)' }}
                                className="mini-column"
                            >
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
                {/* <Grid.Row columns={3} stretched>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('boxesCountGroup')}</label>
                            <Segment
                                style={{ height: 'calc(100% - 22px)' }}
                                className="mini-column"
                            >
                                <Text
                                    name="boxesCount"
                                    text="prepare"
                                    value={form['boxesCount']}
                                    onChange={onChange}
                                />
                                <Text
                                    name="confirmedBoxesCount"
                                    text="fact"
                                    value={form['confirmedBoxesCount']}
                                    onChange={onChange}
                                />
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('weigth')}</label>
                            <Segment
                                style={{ height: 'calc(100% - 22px)' }}
                                className="mini-column"
                            >
                                <Text
                                    name="weightKg"
                                    text="planWeigth"
                                    value={form['weightKg']}
                                    onChange={onChange}
                                />
                                <Text
                                    name="actualWeightKg"
                                    text="factWeigth"
                                    value={form['actualWeightKg']}
                                    onChange={onChange}
                                />
                            </Segment>
                        </Form.Field>
                    </Grid.Column>
                </Grid.Row>*/}
            </Grid>
        </Form>
    );
};

export default Information;
