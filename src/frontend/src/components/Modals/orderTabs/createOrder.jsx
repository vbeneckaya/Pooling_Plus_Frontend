import React, { useEffect } from 'react';
import { Form, Grid } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { valuesListSelector } from '../../../ducks/lookup';
import FormField from '../../BaseComponents';
import {BIG_TEXT_TYPE, DATE_TYPE, SELECT_TYPE, TEXT_TYPE} from '../../../constants/columnTypes';

const CreateOrder = ({form = {}, onChange, isNotUniqueNumber, uniquenessNumberCheck, error}) => {
    const { t } = useTranslation();
    const soldToList = useSelector(state => valuesListSelector(state, 'soldTo')) || [];
    const shippingWarehouseList =
        useSelector(state => valuesListSelector(state, 'shippingWarehousesForOrderCreation')) || [];

    const handleChangeSoldTo = (e, {name, value}) => {
        const item = soldToList.find(item => item.value === value) || {};
        onChange(e, {
            name,
            value,
        });
        onChange(e, {name: 'clientName', value: item.warehouseName});
        onChange(e, {name: 'deliveryAddress', value: item.address});
    };

    const handleChangeShippingWarehouseId = (e, {name, value}) => {
        const item = shippingWarehouseList.find(item => item.value === value) || {};

        onChange(e, {
            name,
            value,
        });

        onChange(e, {name: 'shippingAddress', value: item.address});
    };

    return (
        <Form className="tabs-card">
            <Grid>
                <Grid.Row columns={4}>
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
                            isRequired
                            error={error["clientOrderNumber"]}
                            value={form['clientOrderNumber']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="orderDate"
                            type={DATE_TYPE}
                            isRequired
                            error={error["orderDate"]}
                            value={form['orderDate']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="payer"
                            type={TEXT_TYPE}
                            error={error["payer"]}
                            value={form['payer']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={4}>
                    <Grid.Column>
                        <FormField
                            name="shippingWarehouseId"
                            type={SELECT_TYPE}
                            value={form['shippingWarehouseId']}
                            error={error["shippingWarehouseId"]}
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
                            error={error["soldTo"]}
                            source="soldTo"
                            onChange={handleChangeSoldTo}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="clientName"
                            type={TEXT_TYPE}
                            isDisabled
                            error={error['clientName']}
                            value={form['clientName']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="deliveryDate"
                            type={DATE_TYPE}
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
                            value={form['shippingAddress']}
                            error={form['shippingAddress']}
                            isDisabled
                            rows={2}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="deliveryAddress"
                            type={BIG_TEXT_TYPE}
                            value={form['deliveryAddress']}
                            error={error['deliveryAddress']}
                            isDisabled
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
