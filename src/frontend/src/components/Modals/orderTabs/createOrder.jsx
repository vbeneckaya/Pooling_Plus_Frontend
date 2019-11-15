import React, { useEffect } from 'react';
import { Form, Grid } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';
import Text from '../../BaseComponents/Text';
import Date from '../../BaseComponents/Date';
import TextArea from '../../BaseComponents/TextArea';
import Select from '../../BaseComponents/Select';
import { useSelector } from 'react-redux';
import { valuesListSelector } from '../../../ducks/lookup';
import FormField from '../../BaseComponents';
import {BIG_TEXT_TYPE, DATE_TYPE, SELECT_TYPE, TEXT_TYPE} from '../../../constants/columnTypes';

const CreateOrder = ({ form = {}, onChange, isNotUniqueNumber, uniquenessNumberCheck }) => {
    const { t } = useTranslation();
    const valuesList = useSelector(state => valuesListSelector(state, 'soldTo')) || [];

    const handleChangeSoldTo = (e, {name, value}) => {
        const item = valuesList.find(item => item.value === value) || {};
        onChange(null, {
            name,
            value,
            clientName: item.warehouseName,
            deliveryAddress: item.address,
        });
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
                            error={isNotUniqueNumber}
                            errorText={isNotUniqueNumber && t('number_already_exists')}
                            onBlur={uniquenessNumberCheck}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="clientOrderNumber"
                            type={TEXT_TYPE}
                            isRequired
                            value={form['clientOrderNumber']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="orderDate"
                            type={DATE_TYPE}
                            isRequired
                            value={form['orderDate']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="payer"
                            type={TEXT_TYPE}
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
                            source="shippingWarehouses"
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="soldTo"
                            type={SELECT_TYPE}
                            isRequired
                            value={form['soldTo']}
                            source="soldTo"
                            onChange={handleChangeSoldTo}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="clientName"
                            type={TEXT_TYPE}
                            isDisabled
                            value={form['clientName']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="deliveryDate"
                            type={DATE_TYPE}
                            value={form['deliveryDate']}
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
