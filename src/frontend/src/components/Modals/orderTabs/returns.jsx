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
                        <Text
                            name="returnShippingAccountNo"
                            value={form['returnShippingAccountNo']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <Text name="majorAdoptionNumber" value={form['majorAdoptionNumber']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <Text name="Расхождение(шт)" value={form['addressFrom']} onChange={onChange} />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns='equal' stretched>
                    <Grid.Column width={5}>
                        <Date name="plannedReturnDate" value={form['plannedReturnDate']} onChange={onChange} />
                        <Date name="actualReturnDate" value={form['actualReturnDate']} onChange={onChange} />
                    </Grid.Column>
                    <Grid.Column>
                        <TextArea name="returnInformation" value={form['returnInformation']} rows={5} onChange={onChange} />
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        </Form>
    );
};

export default Returns;
