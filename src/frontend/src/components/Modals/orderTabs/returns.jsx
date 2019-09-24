import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid } from 'semantic-ui-react';
import TextArea from '../../BaseComponents/TextArea';
import Text from '../../BaseComponents/Text';
import Date from "../../BaseComponents/Date";
import Number from "../../BaseComponents/Number";

const Returns = ({ form, onChange }) => {
    const { t } = useTranslation();

    return (
        <Form>
            <Grid>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <TextArea
                            name="addressFrom"
                            value={form['addressFrom']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Text name="addressFrom" value={form['addressFrom']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Text name="addressFrom" value={form['addressFrom']} onChange={onChange} />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={3}>
                    <Grid.Column>
                        <Date name="addressFrom" value={form['addressFrom']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Date name="addressFrom" value={form['addressFrom']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Number name="addressFrom" value={form['addressFrom']} onChange={onChange} />
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        </Form>
    );
};

export default Returns;
