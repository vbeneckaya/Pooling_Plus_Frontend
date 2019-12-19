import React, {useEffect, useCallback, useRef} from 'react';
import {useTranslation} from 'react-i18next';
import {Form, Grid, Segment} from 'semantic-ui-react';
import {useDispatch, useSelector} from 'react-redux';
import {getLookupRequest, valuesListSelector} from '../../../../ducks/lookup';
import FormField from '../../../../components/BaseComponents';
import {
    BIG_TEXT_TYPE,
    DATE_TYPE,
    NUMBER_TYPE,
    SELECT_TYPE,
    SOLD_TO_TYPE,
    TEXT_TYPE,
} from '../../../../constants/columnTypes';
import {addError, clearError} from '../../../../ducks/gridCard';

const Information = ({
                         form,
                         onChange,
                         isNotUniqueNumber,
                         uniquenessNumberCheck,
                         settings,
                         error,
                     }) => {
    const {t} = useTranslation();

    const handleChangeSoldTo = useCallback((e, {name, value}) => {
        onChange(e, {
            name,
            value: value && value.value ? {
                value: value.value,
                name: value.value
            } : null,
        });
        onChange(e, {name: 'clientName', value: value ? value.warehouseName : null});
        onChange(e, {name: 'deliveryAddress', value: value ? value.address : null});
    }, []);


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
                                                type={TEXT_TYPE}
                                                value={form['orderNumber']}
                                                error={(isNotUniqueNumber && t('number_already_exists')) || error['orderNumber']}
                                                onBlur={uniquenessNumberCheck}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="clientOrderNumber"
                                                type={TEXT_TYPE}
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
                                                type={DATE_TYPE}
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
                                                type={SELECT_TYPE}
                                                settings={settings['clientId']}
                                                isDisabled
                                                value={form['clientId']}
                                                error={error['clientId']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="pickingTypeId"
                                                value={form['pickingTypeId']}
                                                type={SELECT_TYPE}
                                                settings={settings['pickingTypeId']}
                                                error={error['pickingTypeId']}
                                                source="pickingTypes"
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
                                                        type={NUMBER_TYPE}
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
                                                name="shippingAddress"
                                                value={form['shippingAddress']}
                                                rows={2}
                                                isDisabled
                                                type={BIG_TEXT_TYPE}
                                                error={error['shippingAddress']}
                                                settings={settings['shippingAddress']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="deliveryAddress"
                                                value={form['deliveryAddress']}
                                                error={error['deliveryAddress']}
                                                isDisabled
                                                type={BIG_TEXT_TYPE}
                                                settings={settings['deliveryAddress']}
                                                rows={2}
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
                                                type={DATE_TYPE}
                                                settings={settings['shippingDate']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column className="mini-column">
                                            <FormField
                                                name="deliveryDate"
                                                value={form['deliveryDate']}
                                                error={error['deliveryDate']}
                                                type={DATE_TYPE}
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
                                                value={form["Количество штук в накладной"]}
                                                error={error["Количество штук в накладной"]}
                                                type={NUMBER_TYPE}
                                                settings={settings["Количество штук в накладной"]}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="Стоимость товара в накладной"
                                                value={form["Стоимость товара в накладной"]}
                                                error={error["Стоимость товара в накладной"]}
                                                type={NUMBER_TYPE}
                                                settings={settings["Стоимость товара в накладной"]}
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
                                                                    text="prepare"
                                                                    value={form['boxesCount']}
                                                                    error={error['boxesCount']}
                                                                    type={NUMBER_TYPE}
                                                                    settings={settings['boxesCount']}
                                                                    onChange={onChange}
                                                                />
                                                            </Grid.Column>
                                                        </Grid.Row>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="confirmedBoxesCount"
                                                                    text="plan"
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
                                                <label>{t('palletsCountGroup')}</label>
                                                <Segment>
                                                    <Grid>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="palletsCount"
                                                                    text="prepare"
                                                                    value={form['palletsCount']}
                                                                    error={error['palletsCount']}
                                                                    type={NUMBER_TYPE}
                                                                    settings={settings['palletsCount']}
                                                                    onChange={onChange}
                                                                />
                                                            </Grid.Column>
                                                        </Grid.Row>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="confirmedPalletsCount"
                                                                    text="plan"
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
                                                                    settings={settings['factWeigth']}
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
