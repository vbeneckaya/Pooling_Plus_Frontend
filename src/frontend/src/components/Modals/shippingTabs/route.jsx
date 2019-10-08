import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid, Tab, Table } from 'semantic-ui-react';
import Select from '../../BaseComponents/Select';
import TextArea from '../../BaseComponents/TextArea';
import DateTime from '../../BaseComponents/DateTime';
import Date from "../../BaseComponents/Date";
import State from "../../BaseComponents/State";
import Text from "../../BaseComponents/Text";


const Route = ({ name, form = {}, onChange }) => {
    const { t } = useTranslation();
console.log('form', form);
    return (
        <Form style={{paddingLeft: "12px"}}>
            <Grid>
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <DateTime name="plannedDate" value={form["plannedDate"]} onChange={onChange}/>
                    </Grid.Column>
                    <Grid.Column>
                        <State name="vehicleStatus" value={form["vehicleStatus"]} source="vehicleState" onChange={onChange}/>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <DateTime name="arrivalTime" value={form["arrivalTime"]} onChange={onChange}/>
                    </Grid.Column>
                    <Grid.Column>
                        <DateTime name="departureTime" value={form["departureTime"]} onChange={onChange}/>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column width={16}>
                    <TextArea name="address" value={form["address"]} onChange={onChange}/>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column width={18}>
                        <TextArea name="deviationReasonsComments" value={form["deviationReasonsComments"]} onChange={onChange}/>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column width={8}>
                        <Text name="trucksDowntime" value={form["trucksDowntime"]} onChange={onChange}/>
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        </Form>
    );
};

export default Route;
