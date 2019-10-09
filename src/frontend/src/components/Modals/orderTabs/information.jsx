import React, {useEffect, useState} from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid, Segment } from 'semantic-ui-react';
import Text from '../../BaseComponents/Text';
import State from '../../BaseComponents/State';
import Date from '../../BaseComponents/Date';
import Select from '../../BaseComponents/Select';
import TextArea from '../../BaseComponents/TextArea';
import {useSelector} from "react-redux";
import {valuesListSelector} from "../../../ducks/lookup";

const Information = ({ form, onChange }) => {
    const { t } = useTranslation();
    let [error, setError] = useState(false);

    const valuesList = useSelector(state => valuesListSelector(state, 'soldTo')) || [];

    useEffect(
        () => {
            const item = valuesList.find(item => item.value === form.soldTo) || {};
            onChange(null, { name: 'clientName', value: item.warehouseName });
        },
        [form.soldTo],
    );

    useEffect(
        () => {
            const item = valuesList.find(item => item.value === form.soldTo) || {};
            onChange(null, {name: 'deliveryAddress', value: item.address});
        },
        [form.clientName],
    );

    useEffect(() => {
        console.log('valuesList', valuesList.find(item => item.value === form.soldTo), form.soldTo);
        if (form.soldTo && !valuesList.find(item => item.value === form.soldTo)) {
            setError(true)
        } else {
            setError(false)
        }
    }, [valuesList, form.soldTo]);

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
                                                isDisabled
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
                                                disabled
                                                isTranslate
                                                source="orderType"
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <Select
                                                name="soldTo"
                                                value={form['soldTo']}
                                                errorText={t('soldTo_error')}
                                                textValue={error && form['soldTo']}
                                                error={error}
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
                                                value={form['shippingAddress']}
                                                rows={2}
                                                isDisabled
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <TextArea
                                                name="deliveryAddress"
                                                value={form['deliveryAddress']}
                                                isDisabled
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
