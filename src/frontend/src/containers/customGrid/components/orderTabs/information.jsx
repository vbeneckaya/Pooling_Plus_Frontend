import React, {useEffect, useCallback, useRef, useMemo} from 'react';
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
import TextArea from "../../../../components/BaseComponents/Text";

const Information = ({
    form,
    onChange,
    onBlur,
    //isNotUniqueNumber,
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

    const extSearchParams = useMemo(() => ({
        filter: form['clientId'] ? form['clientId'].value : undefined,
    }), [form['clientId']]);

    return (
        <Form className="tabs-card">
            <Grid>
                <Grid.Row>
                    <Grid.Column>
                        <Form.Field>
                            <label>{t('general info')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <FormField
                                                name="orderNumber"
                                                type={TEXT_TYPE}
                                                source={getColumn('orderNumber').source}
                                                value={!!form['orderNumber'] ? form['orderNumber'].value : form['orderNumber']}
                                                error={(t('number_already_exists')) && error['orderNumber']}
                                                onBlur={uniquenessNumberCheck}
                                                onChange={onChange}
                                            />
                                            <FormField
                                                name="clientOrderNumber"
                                                type={getColumn('clientOrderNumber').type}
                                                source={getColumn('clientOrderNumber').source}
                                                settings={settings['clientOrderNumber']}
                                                value={form['clientOrderNumber']}
                                                error={error['clientOrderNumber']}
                                                onBlur={onBlur}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
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
                                    {/*<Grid.Row columns={3}>
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
                                    </Grid.Row>*/}
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
                                                subTitle={form['shippingAddress']}
                                                settings={settings['shippingWarehouseId']}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="deliveryWarehouseId"
                                                value={form['deliveryWarehouseId']}
                                                error={error['deliveryWarehouseId']}
                                                subTitle={form['deliveryAddress']}
                                                type={getColumn('deliveryWarehouseId').type}
                                                source={getColumn('deliveryWarehouseId').source}
                                                extSearchParams={extSearchParams}
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
                            <label>{t('Products on the invoice')}</label>
                            <Segment>
                                <Grid>
                                    <Grid.Row columns={3}>
                                        <Grid.Column>
                                            <FormField
                                                name="articlesCount"
                                                value={form['articlesCount']}
                                                error={error['articlesCount']}
                                                type={NUMBER_TYPE}
                                                settings={settings['articlesCount']}
                                                onBlur={onBlur}
                                                onChange={onChange}

                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="orderAmountExcludingVAT"
                                                value={form['orderAmountExcludingVAT']}
                                                error={error['orderAmountExcludingVAT']}
                                                type={NUMBER_TYPE}
                                                settings={settings['orderAmountExcludingVAT']}
                                                onBlur={onBlur}
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
                                                                    onBlur={onBlur}
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
                                                                    onBlur={onBlur}
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
                                                                    onBlur={onBlur}
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
                                                                    onBlur={onBlur}                                                                    
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
                                                <label>{t('weight')}</label>
                                                <Segment>
                                                    <Grid>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="weightKg"
                                                                    text="planWeight"
                                                                    value={form['weightKg']}
                                                                    error={error['weightKg']}
                                                                    type={NUMBER_TYPE}
                                                                    settings={settings['weightKg']}
                                                                    onBlur={onBlur}
                                                                    onChange={onChange}
                                                                />
                                                            </Grid.Column>
                                                        </Grid.Row>
                                                        <Grid.Row>
                                                            <Grid.Column>
                                                                <FormField
                                                                    name="factWeight"
                                                                    text="factWeight"
                                                                    value={form['factWeight']}
                                                                    error={error['factWeight']}
                                                                    type={NUMBER_TYPE}
                                                                    settings={
                                                                        settings['factWeight']
                                                                    }
                                                                    onChange={onChange}
                                                                    onBlur={onBlur}
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
                            <label>{t('weight')}</label>
                            <Segment
                                style={{height: 'calc(100% - 22px)'}}
                                className="mini-column"
                            >
                                <Grid>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <FormField
                                                name="weightKg"
                                                text="planWeight"
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
                                                text="factWeight"
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
