import React, { useEffect, useCallback, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid, Segment } from 'semantic-ui-react';
import { useDispatch, useSelector } from 'react-redux';
import { getLookupRequest, valuesListSelector } from '../../../../ducks/lookup';
import FormField from '../../../../components/BaseComponents';
import {
    BIG_TEXT_TYPE,
    DATE_TYPE,
    NUMBER_TYPE,
    SELECT_TYPE,
    SOLD_TO_TYPE,
    TEXT_TYPE,
} from '../../../../constants/columnTypes';
import { addError, clearError } from '../../../../ducks/gridCard';

const Information = ({
    form,
    onChange,
    isNotUniqueNumber,
    uniquenessNumberCheck,
    settings,
    error,
    columns,
}) => {
    const { t } = useTranslation();

    const getColumn = (key) => {
        const column = columns.find(item => item.name === key);
        return column ? column : {};
    };

    return (
        <Form className="tabs-card">
            <Grid>
                <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('general info')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={3}>
                                        <Grid.Column>
                                            <FormField
                                                name="orderNumber"
                                                type={getColumn('orderNumber').type}
                                                source={getColumn('orderNumber').source}
                                                value={form['orderNumber']}
                                                error={
                                                    (isNotUniqueNumber &&
                                                        t('number_already_exists')) ||
                                                    error['orderNumber']
                                                }
                                                onBlur={uniquenessNumberCheck}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="clientOrderNumber"
                                                type={getColumn('clientOrderNumber').type}
                                                source={getColumn('clientOrderNumber').source}
                                                settings={settings['clientOrderNumber']}
                                                value={form['clientOrderNumber']}
                                                error={error['clientOrderNumber']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="orderDate"
                                                value={form['orderDate']}
                                                type={getColumn('orderDate').type}
                                                source={getColumn('orderDate').source}
                                                settings={settings['orderDate']}
                                                error={error['orderDate']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row columns={3}>
                                        <Grid.Column>
                                            <FormField
                                                name="clientId"
                                                type={getColumn('clientId').type}
                                                source={getColumn('clientId').source}
                                                settings={settings['clientId']}
                                                value={form['clientId']}
                                                error={error['clientId']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="pickingTypeId"
                                                value={form['pickingTypeId']}
                                                type={getColumn('pickingTypeId').type}
                                                source={getColumn('pickingTypeId').source}
                                                settings={settings['pickingTypeId']}
                                                error={error['pickingTypeId']}
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
                                                        type={getColumn('temperatureMin').type}
                                                        source={getColumn('temperatureMin').source}
                                                        error={error['temperatureMin']}
                                                        settings={settings['temperatureMin']}
                                                        onChange={onChange}
                                                    />
                                                    <label>{t('to')}</label>
                                                    <FormField
                                                        noLabel
                                                        name="temperatureMax"
                                                        value={form['temperatureMax']}
                                                        error={error['temperatureMax']}
                                                        type={getColumn('temperatureMax').type}
                                                        source={getColumn('temperatureMax').source}
                                                        settings={settings['temperatureMax']}
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
                                            <FormField
                                                name="shippingWarehouseId"
                                                value={form['shippingWarehouseId']}
                                                type={getColumn('shippingWarehouseId').type}
                                                source={getColumn('shippingWarehouseId').source}
                                                error={error['shippingWarehouseId']}
                                                settings={settings['shippingWarehouseId']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="deliveryWarehouseId"
                                                value={form['deliveryWarehouseId']}
                                                error={error['deliveryWarehouseId']}
                                                type={getColumn('deliveryWarehouseId').type}
                                                source={getColumn('deliveryWarehouseId').source}
                                                settings={settings['deliveryWarehouseId']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row columns={2}>
                                        <Grid.Column className="mini-column">
                                            <FormField
                                                name="shippingDate"
                                                value={form['shippingDate']}
                                                error={error['shippingDate']}
                                                type={getColumn('shippingDate').type}
                                                source={getColumn('shippingDate').source}
                                                settings={settings['shippingDate']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column className="mini-column">
                                            <FormField
                                                name="deliveryDate"
                                                value={form['deliveryDate']}
                                                error={error['deliveryDate']}
                                                type={getColumn('deliveryDate').type}
                                                source={getColumn('deliveryDate').source}
                                                settings={settings['deliveryDate']}
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
                            <label>{t('Продукция в накладной')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={3}>
                                        <Grid.Column>
                                            <FormField
                                                name="Количество штук в накладной"
                                                value={form['Количество штук в накладной']}
                                                error={error['Количество штук в накладной']}
                                                type={NUMBER_TYPE}
                                                settings={settings['Количество штук в накладной']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="Стоимость товара в накладной"
                                                value={form['Стоимость товара в накладной']}
                                                error={error['Стоимость товара в накладной']}
                                                type={NUMBER_TYPE}
                                                settings={settings['Стоимость товара в накладной']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="orderType"
                                                value={form['orderType']}
                                                type={SELECT_TYPE}
                                                settings={settings['orderType']}
                                                error={error['orderType']}
                                                isTranslate
                                                source="orderType"
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row columns={3}>
                                        <Grid.Column>
                                            <Form.Field>
                                                <label>{t('boxesCountGroup')}</label>
                                                <Segment>
                                                    <Grid>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="boxesCount"
                                                                    text="plan"
                                                                    value={form['boxesCount']}
                                                                    error={error['boxesCount']}
                                                                    type={NUMBER_TYPE}
                                                                    settings={
                                                                        settings['boxesCount']
                                                                    }
                                                                    onChange={onChange}
                                                                />
                                                            </Grid.Column>
                                                        </Grid.Row>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="confirmedBoxesCount"
                                                                    text="fact"
                                                                    value={
                                                                        form['confirmedBoxesCount']
                                                                    }
                                                                    error={
                                                                        error['confirmedBoxesCount']
                                                                    }
                                                                    type={NUMBER_TYPE}
                                                                    settings={
                                                                        settings[
                                                                            'confirmedBoxesCount'
                                                                        ]
                                                                    }
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
                                                <label>{t('palletsCountGroup')}</label>
                                                <Segment>
                                                    <Grid>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="palletsCount"
                                                                    text="plan"
                                                                    value={form['palletsCount']}
                                                                    error={error['palletsCount']}
                                                                    type={NUMBER_TYPE}
                                                                    settings={
                                                                        settings['palletsCount']
                                                                    }
                                                                    onChange={onChange}
                                                                />
                                                            </Grid.Column>
                                                        </Grid.Row>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="confirmedPalletsCount"
                                                                    text="fact"
                                                                    value={
                                                                        form[
                                                                            'confirmedPalletsCount'
                                                                        ]
                                                                    }
                                                                    error={
                                                                        error[
                                                                            'confirmedPalletsCount'
                                                                        ]
                                                                    }
                                                                    type={NUMBER_TYPE}
                                                                    settings={
                                                                        settings[
                                                                            'confirmedPalletsCount'
                                                                        ]
                                                                    }
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
                                                <Segment>
                                                    <Grid>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="weightKg"
                                                                    text="planWeigth"
                                                                    value={form['weightKg']}
                                                                    error={error['weightKg']}
                                                                    type={NUMBER_TYPE}
                                                                    settings={settings['weightKg']}
                                                                    onChange={onChange}
                                                                />
                                                            </Grid.Column>
                                                        </Grid.Row>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="factWeigth"
                                                                    text="factWeigth"
                                                                    value={form['factWeigth']}
                                                                    error={error['factWeigth']}
                                                                    type={NUMBER_TYPE}
                                                                    settings={
                                                                        settings['factWeigth']
                                                                    }
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
                            </Segment>
                        </Form.Field>
                    </Grid.Column>

                    {/*<Grid.Column>
                        <Form.Field>
                            <label>{t('palletsCountGroup')}</label>
                            <Segment className="mini-column">
                                <Grid>
                                    <Grid.Row columns={4}>
                                        <Grid.Column>
                                            <FormField
                                                name="palletsCount"
                                                text="prepare"
                                                error={error['palletsCount']}
                                                value={form['palletsCount']}
                                                type={NUMBER_TYPE}
                                                settings={settings['palletsCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="actualPalletsCount"
                                                text="plan"
                                                value={form['actualPalletsCount']}
                                                error={error['actualPalletsCount']}
                                                type={NUMBER_TYPE}
                                                settings={settings['actualPalletsCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="confirmedPalletsCount"
                                                text="fact"
                                                value={form['confirmedPalletsCount']}
                                                error={error['confirmedPalletsCount']}
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
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('boxesCountGroup')}</label>
                            <Segment className="mini-column">
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <FormField
                                                name="boxesCount"
                                                text="prepare"
                                                value={form['boxesCount']}
                                                error={error['boxesCount']}
                                                type={NUMBER_TYPE}
                                                settings={settings['boxesCount']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="confirmedBoxesCount"
                                                text="fact"
                                                value={form['confirmedBoxesCount']}
                                                error={error['confirmedBoxesCount']}
                                                type={NUMBER_TYPE}
                                                settings={settings['confirmedBoxesCount']}
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
                                style={{height: 'calc(100% - 22px)'}}
                                className="mini-column"
                            >
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <FormField
                                                name="weightKg"
                                                text="planWeigth"
                                                value={form['weightKg']}
                                                error={error['weightKg']}
                                                type={NUMBER_TYPE}
                                                settings={settings['weightKg']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="actualWeightKg"
                                                text="factWeigth"
                                                value={form['actualWeightKg']}
                                                error={error['actualWeightKg']}
                                                type={NUMBER_TYPE}
                                                settings={settings['actualWeightKg']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                </Grid>
                            </Segment>
                        </Form.Field>
                    </Grid.Column>*/}
                </Grid.Row>
            </Grid>
        </Form>
    );
};

export default Information;
