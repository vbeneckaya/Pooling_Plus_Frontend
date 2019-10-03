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

    return (
        <Form style={{paddingLeft: "12px"}}>
            <Grid>
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <Date/>
                    </Grid.Column>
                    <Grid.Column>
                        <State/>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <DateTime/>
                    </Grid.Column>
                    <Grid.Column>
                        <DateTime/>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column width={16}>
                    <TextArea/>
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
