import React, {useState, useCallback} from 'react';
import {Form, Grid} from 'semantic-ui-react';
import {useTranslation} from 'react-i18next';
import FormField from '../../../../components/BaseComponents';
import {BIG_TEXT_TYPE, DATE_TYPE, SELECT_TYPE, TEXT_TYPE} from '../../../../constants/columnTypes';

const CreateOrder = ({form = {}, onChange, isNotUniqueNumber, uniquenessNumberCheck, error}) => {
    const {t} = useTranslation();

    return (
        <Form className="tabs-card">
            <Grid>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <FormField
                            name="orderNumber"
                            type={TEXT_TYPE}
                            isRequired
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
                            error={error["clientOrderNumber"]}
                            value={form['clientOrderNumber']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="orderDate"
                            type={DATE_TYPE}
                            error={error["orderDate"]}
                            value={form['orderDate']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={2}>
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
                            name="deliveryDate"
                            type={DATE_TYPE}
                            isRequired
                            value={form['deliveryDate']}
                            error={error['deliveryDate']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <FormField
                            name="shippingWarehouseId"
                            type={SELECT_TYPE}
                            source="shippingAddress"
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
                            isRequired
                            /*extSearchParams={{clientId: form['clientId'] ? form['clientId'].value : undefined}}*/
                            source="deliveryAddress"
                            value={form['deliveryWarehouseId']}
                            error={error['deliveryWarehouseId']}
                            rows={2}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        </Form>
    );
};

export default CreateOrder;
