import React from 'react';
import {Form, Grid} from "semantic-ui-react";
import Text from "../../BaseComponents/Text";
import Date from "../../BaseComponents/Date";
import TextArea from "../../BaseComponents/TextArea";
import Select from "../../BaseComponents/Select";

const CreateOrder = ({ form = {}, onChange}) => {
    const handleChangeSoldTo = (e, {name, value}) => {

        onChange(e, {name, value});
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
