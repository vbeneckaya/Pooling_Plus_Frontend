import React, { useEffect } from 'react';
import { Form, Grid } from 'semantic-ui-react';
import { useTranslation } from 'react-i18next';
import Text from '../../BaseComponents/Text';
import Date from '../../BaseComponents/Date';
import TextArea from '../../BaseComponents/TextArea';
import Select from '../../BaseComponents/Select';
import { useSelector } from 'react-redux';
import { valuesListSelector } from '../../../ducks/lookup';

const CreateOrder = ({ form = {}, onChange, isNotUniqueNumber, uniquenessNumberCheck }) => {
    const { t } = useTranslation();
    const valuesList = useSelector(state => valuesListSelector(state, 'soldTo')) || [];

    useEffect(() => {
        const item = valuesList.find(item => item.value === form.soldTo) || {};
        onChange(null, { name: 'clientName', value: item.warehouseName });
    }, [form.soldTo]);

    useEffect(() => {
        const item = valuesList.find(item => item.value === form.soldTo) || {};
        onChange(null, { name: 'deliveryAddress', value: item.address });
    }, [form.clientName]);

    return (
        <Form className="tabs-card">
            <Grid>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <Text
                            name="orderNumber"
                            value={form['orderNumber']}
                            error={isNotUniqueNumber}
                            errorText={isNotUniqueNumber && t('number_already_exists')}
                            onBlur={uniquenessNumberCheck}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Date name="orderDate" value={form['orderDate']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Text name="payer" value={form['payer']} onChange={onChange} />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <Select
                            name="soldTo"
                            value={form['soldTo']}
                            source="soldTo"
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
                        <Date
                            name="deliveryDate"
                            value={form['deliveryDate']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
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
            </Grid>
        </Form>
    );
};

export default CreateOrder;
