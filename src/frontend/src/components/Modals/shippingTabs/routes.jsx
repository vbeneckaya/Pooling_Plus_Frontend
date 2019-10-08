import React from 'react';
import { Form, Grid, Icon, Tab } from 'semantic-ui-react';
import Route from './route';
import {useSelector, useDispatch} from "react-redux";
import {stateColorsSelector} from "../../../ducks/gridList";
import {getLookupRequest, valuesListSelector} from "../../../ducks/lookup";

const Routes = ({ form, onChange }) => {
    const dispatch = useDispatch();
    const points = form.routePoints;
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
                return <Route name={point.warehouseName} form={point} onChange={onChange} />;
            },
        });
    });

    return (
        <div>
            <Tab
                className="all-tabs"
                panes={pointsTabs}
                menu={{ vertical: true }}
                menuPosition="left"
            />
        </div>
    );
};

export default Routes;
