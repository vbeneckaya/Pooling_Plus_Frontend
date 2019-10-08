import React from 'react';
import {Form, Grid} from "semantic-ui-react";
import Text from "../../BaseComponents/Text";
import Date from "../../BaseComponents/Date";
import TextArea from "../../BaseComponents/TextArea";
import Select from "../../BaseComponents/Select";
import {useSelector} from "react-redux";
import {valuesListSelector} from "../../../ducks/lookup";

const CreateOrder = ({ form = {}, onChange}) => {
    const valuesList = useSelector(state => valuesListSelector(state, 'soldTo')) || [];

    const handleChangeSoldTo = (e, {name, value}) => {
        console.log('valuesList', valuesList);
        const item = valuesList.find(item => item.value === value) || {};

        onChange(e, {name, value});
        onChange(e, {name: 'clientName', value: item.warehouseName});
        onChange(e, {name: 'deliveryAddress', value: item.address});

        if (item.pickingTypeId) onChange(e, {name: 'pickingTypeId', value: item.pickingTypeId});
    };

    return (
        <Form className="tabs-card">
            <Grid>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <Text
                            name="orderNumber"
                            value={form['orderNumber']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Date
                            name="orderDate"
                            value={form['orderDate']}
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
                </Grid.Row>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <Select
                            name="soldTo"
                            value={form['soldTo']}
                            source="soldTo"
                            onChange={handleChangeSoldTo}
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
    )
};

export default CreateOrder;
