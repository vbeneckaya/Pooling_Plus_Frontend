import React, { useState } from 'react';
import { Form, Grid, Icon, Tab } from 'semantic-ui-react';
import Route from './route';
import { useSelector, useDispatch } from 'react-redux';
import { stateColorsSelector } from '../../../ducks/gridList';
import { getLookupRequest, valuesListSelector } from '../../../ducks/lookup';

const Routes = ({ form, onChange, routeActiveIndex, tabChange }) => {
    const dispatch = useDispatch();
    const { routePoints: points } = form;
    const stateColors = useSelector(state => valuesListSelector(state, 'vehicleState')) || [];

    if (!stateColors.length) {
        dispatch(
            getLookupRequest({
                name: 'vehicleState',
                isForm: true,
                isSearch: true,
            }),
        );
    }

    const handleChange = (point, index) => {
        points[index] = point;
        onChange(null, {
            name: 'routePoints',
            value: points,
        });
    };

    const pointsTabs = [];

    points.forEach((point, i) => {
        const state = stateColors.find(x => x.name === point.vehicleStatus);
        const color = state ? state.color : 'grey';
        pointsTabs.push({
            menuItem: {
                key: i,
                content: (
                    <label>
                        <Icon color={color} name="circle" />
                        {point.warehouseName}
                    </label>
                ),
            },
            render: () => {
                return (
                    <Route
                        index={i}
                        form={form}
                        point={point}
                        pointChange={handleChange}
                        onChange={onChange}
                    />
                );
            },
        });
    });

    return (
        <div>
            <Tab
                className="all-tabs"
                panes={pointsTabs}
                activeIndex={routeActiveIndex}
                menu={{ vertical: true }}
                menuPosition="left"
                onTabChange={tabChange}
            />
        </div>
    );
};

export default Routes;
