import React, {useMemo} from 'react';
import {Form, Grid, Segment} from 'semantic-ui-react';
import {useTranslation} from 'react-i18next';
import FormField from '../../../../components/BaseComponents';
import {
    DATE_TYPE,
    SELECT_TYPE,
    TEXT_TYPE,
    NUMBER_TYPE,
} from '../../../../constants/columnTypes';

const CreateOrder = ({form = {}, onChange, onBlur, isNotUniqueNumber, uniquenessNumberCheck, error}) => {
    const {t} = useTranslation();

    const extSearchParams = useMemo(() => ({
        filter: form['clientId'] ? form['clientId'].value : undefined,
    }), [form['clientId']]);

    return (
        <Form className="tabs-card">
            <Grid>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <FormField
                                name="clientId"
                                type={SELECT_TYPE}
                                source="clients"
                                isRequired
                                error={error['clientId']}
                                value={form['clientId']}
                                onChange={onChange}
                            />
                    </Grid.Column>
                    <Grid.Column>
                    <FormField
                            name="orderNumber"
                            type={TEXT_TYPE}
                            isRequired
                            value={!!form['orderNumber'] ? form['orderNumber'].value : form['orderNumber']}
                            error={(isNotUniqueNumber && t('number_already_exists')) || error['orderNumber']}
                            onBlur={uniquenessNumberCheck}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="clientOrderNumber"
                            type={TEXT_TYPE}
                            error={error["clientOrderNumber"]}
                            value={form['clientOrderNumber']}
                            onChange={onChange}
                            onBlur={onBlur}
                        />                        
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <FormField
                            name="palletsCount"
                            type={NUMBER_TYPE}
                            source="palletsCount"
                            value={form['palletsCount']}
                            error={error['palletsCount']}
                            rows={2}
                            onChange={onChange}
                            onBlur={onBlur}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="orderAmountExcludingVAT"
                            type={NUMBER_TYPE}
                            value={form['orderAmountExcludingVAT']}
                            error={error['orderAmountExcludingVAT']}
                            rows={2}
                            onChange={onChange}
                            onBlur={onBlur}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="weightKg"
                            type={NUMBER_TYPE}
                            source="weightKg"
                            value={form['weightKg']}
                            error={error['weightKg']}
                            rows={2}
                            onChange={onChange}
                            onBlur={onBlur}
                        />
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
                                                type={SELECT_TYPE}
                                                source="shippingWarehouses"
                                                isRequired
                                                value={form['shippingWarehouseId']}
                                                error={error['shippingWarehouseId']}
                                                rows={2}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="deliveryWarehouseId"
                                                type={SELECT_TYPE}
                                                isDisabled={!form['clientId']}
                                                isRequired
                                                extSearchParams={extSearchParams}
                                                source="warehouses"
                                                value={form['deliveryWarehouseId']}
                                                error={error['deliveryWarehouseId']}
                                                rows={2}
                                                onChange={onChange}
                                            />
                                        </Grid.Column>
                                    </Grid.Row>
                                    <Grid.Row columns={2}>
                                        <Grid.Column>
                                            <FormField
                                                    name="shippingDate"
                                                    type={DATE_TYPE}
                                                    value={form['shippingDate']}
                                                    error={error['shippingDate']}
                                                    onChange={onChange}
                                                />
                                        </Grid.Column>
                                        <Grid.Column>
                                            <FormField
                                                name="deliveryDate"
                                                type={DATE_TYPE}
                                                isRequired
                                                value={form['deliveryDate']}
                                                error={error['deliveryDate']}
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

export default CreateOrder;
