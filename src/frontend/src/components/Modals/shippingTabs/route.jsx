import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid, Tab, Table } from 'semantic-ui-react';
import Select from '../../BaseComponents/Select';
import TextArea from '../../BaseComponents/TextArea';
import DateTime from '../../BaseComponents/DateTime';
import Date from '../../BaseComponents/Date';
import State from '../../BaseComponents/State';
import Text from '../../BaseComponents/Text';
import Number from '../../BaseComponents/Number';
import FormField from "../../BaseComponents";
import {BIG_TEXT_TYPE, DATE_TIME_TYPE, NUMBER_TYPE, STATE_TYPE} from "../../../constants/columnTypes";

const Route = ({name, form = {}, point = {}, onChange, pointChange, index, settings}) => {
    const { t } = useTranslation();

    const handleChange = (e, { name, value }) => {
        pointChange(
            {
                ...point,
                [name]: value,
            },
            index,
        );
    };

    return (
        <Form style={{ paddingLeft: '12px' }}>
            <Grid>
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <FormField
                            name="plannedDate"
                            text={index === 0 ? 'plannedDate_loading' : 'plannedDate_delivery'}
                            value={point['plannedDate']}
                            type={DATE_TIME_TYPE}
                            onChange={handleChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="vehicleStatus"
                            value={point['vehicleStatus']}
                            source="vehicleState"
                            type={STATE_TYPE}
                            onChange={handleChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <FormField
                            name="arrivalTime"
                            value={point['arrivalTime']}
                            type={DATE_TIME_TYPE}
                            onChange={handleChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <FormField
                            name="departureTime"
                            value={point['departureTime']}
                            type={DATE_TIME_TYPE}
                            onChange={handleChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column width={16}>
                        <FormField name="address" value={point['address']} type={BIG_TEXT_TYPE}
                                   onChange={handleChange}/>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column width={18}>
                        <FormField
                            name="deviationReasonsComments"
                            type={BIG_TEXT_TYPE}
                            value={form['deviationReasonsComments']}
                            settings={settings["deviationReasonsComments"]}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column width={8}>
                        <FormField
                            name="trucksDowntime"
                            value={point['trucksDowntime']}
                            type={NUMBER_TYPE}
                            onChange={handleChange}
                        />
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        </Form>
    );
};

export default Route;
