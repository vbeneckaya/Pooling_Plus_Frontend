import React, { useEffect, useRef, useCallback } from 'react';
import { Form, Grid } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';
import FormField from '../../../../components/BaseComponents';
import {
    BIG_TEXT_TYPE,
    DATE_TYPE,
    SELECT_TYPE,
    TEXT_TYPE,
} from '../../../../constants/columnTypes';

const CreateOrder = ({ form = {}, onChange, isNotUniqueNumber, uniquenessNumberCheck, error }) => {
    const { t } = useTranslation();

    const handleChangeSoldTo = useCallback((e, { name, value }) => {
        onChange(e, {
            name,
            value:
                value && value.value
                    ? {
                          value: value.value,
                          name: value.value,
                      }
                    : null,
        });
        onChange(e, { name: 'clientName', value: value ? value.warehouseName : null });
        onChange(e, { name: 'deliveryAddress', value: value ? value.address : null });
    }, []);

    const handleChangeShippingWarehouseId = useCallback((e, { name, value }) => {
        onChange(e, {
            name,
            value,
        });

        onChange(e, { name: 'shippingAddress', value: value ? value.address : null });
    }, []);

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
                            error={
                                (isNotUniqueNumber && t('number_already_exists')) ||
                                error['orderNumber']
                            }
                            onBlur={uniquenessNumberCheck}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="clientOrderNumber"
                            type={TEXT_TYPE}
                            error={error['clientOrderNumber']}
                            value={form['clientOrderNumber']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="orderDate"
                            type={DATE_TYPE}
                            error={error['orderDate']}
                            value={form['orderDate']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    {/* <Grid.Column>
                        <FormField
                            name="payer"
                            type={TEXT_TYPE}
                            error={error['payer']}
                            value={form['payer']}
                            onChange={onChange}
                        />
                    </Grid.Column>*/}
                </Grid.Row>
                <Grid.Row columns={2}>
                    {/*<Grid.Column>
                        <FormField
                            name="shippingWarehouseId"
                            type={SELECT_TYPE}
                            value={form['shippingWarehouseId']}
                            error={error['shippingWarehouseId']}
                            source="shippingWarehousesForOrderCreation"
                            onChange={handleChangeShippingWarehouseId}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="soldTo"
                            type={SELECT_TYPE}
                            isRequired
                            value={form['soldTo']}
                            error={error['soldTo']}
                            source="soldTo"
                            onChange={handleChangeSoldTo}
                        />
                    </Grid.Column>*/}
                    <Grid.Column>
                        <FormField
                            name="clientId"
                            type={SELECT_TYPE}
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
                            name="shippingAddress"
                            type={BIG_TEXT_TYPE}
                            isRequired
                            value={form['shippingAddress']}
                            error={error['shippingAddress']}
                            rows={2}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="deliveryAddress"
                            type={BIG_TEXT_TYPE}
                            isRequired
                            value={form['deliveryAddress']}
                            error={error['deliveryAddress']}
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
