import React from 'react';
import { Form, Grid, Icon, Tab } from 'semantic-ui-react';
import Route from './route';
import {useSelector} from "react-redux";
import {stateColorsSelector} from "../../../ducks/gridList";

const Routes = ({ form, onChange }) => {
    const points = [
        {
            name: 'РЦ Томилино',
            status: '',
            color: 'green'
        },
        {
            name: 'РЦ Химки',
            status: '',
            color: 'orange'
        },
        {
            name: 'Дикси Долгопрудный',
            status: '',
            color: 'grey'
        },
        {
            name: 'Перекресток Ростокино',
            status: '',
            color: 'grey'
        },
    ];


    const pointsTabs = [];

    points.forEach((point, i) => {
        pointsTabs.push({
            menuItem: {
                key: i,
                content: (
                    <label>
                        <Icon color={point.color} name="circle" />
                        {point.name}
                    </label>
                ),
            },
            render: () => {
                return <Route name={point.name} form={form} onChange={onChange} />;
            },
        });
    });

    return (
        <div>
            <Tab
                className="all-tabs"
                panes={pointsTabs}
                menu={{ fluid: true, vertical: true }}
                menuPosition="left"
            />
        </div>
    );
};

export default Routes;
