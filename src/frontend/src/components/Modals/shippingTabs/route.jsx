import React from 'react';
import { useTranslation } from 'react-i18next';
import { Form, Grid, Tab, Table } from 'semantic-ui-react';
import Select from '../../BaseComponents/Select';
import TextArea from '../../BaseComponents/TextArea';
import DateTime from '../../BaseComponents/DateTime';
import Date from '../../BaseComponents/Date';
import State from '../../BaseComponents/State';
import Text from '../../BaseComponents/Text';
import Number from "../../BaseComponents/Number";

const Route = ({ name, form = {}, point = {}, onChange, pointChange, index }) => {
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
                        <DateTime
                            name="plannedDate"
                            text={index === 0 ? 'plannedDate_loading' : 'plannedDate_delivery'}
                            value={point['plannedDate']}
                            onChange={handleChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <State
                            name="vehicleStatus"
                            value={point['vehicleStatus']}
                            source="vehicleState"
                            onChange={handleChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={2}>
                    <Grid.Column>
                        <DateTime
                            name="arrivalTime"
                            value={point['arrivalTime']}
                            onChange={handleChange}
                        />
                    </Grid.Column>
                    <Grid.Column>
                        <DateTime
                            name="departureTime"
                            value={point['departureTime']}
                            onChange={handleChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column width={16}>
                        <TextArea name="address" value={point['address']} onChange={handleChange} />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column width={18}>
                        <TextArea
                            name="deviationReasonsComments"
                            value={form['deviationReasonsComments']}
                            onChange={onChange}
                        />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row columns={1}>
                    <Grid.Column width={8}>
                        <Number
                            name="trucksDowntime"
                            value={point['trucksDowntime']}
                            onChange={handleChange}
                        />
                    </Grid.Column>
                </Grid.Row>
            </Grid>
        </Form>
    );
};

export default Route;
