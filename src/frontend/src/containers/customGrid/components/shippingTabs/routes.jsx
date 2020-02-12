import React, {useCallback, useEffect} from 'react';
import {Icon, Tab} from 'semantic-ui-react';
import Route from './route';
import {useDispatch, useSelector} from 'react-redux';
import {getLookupRequest, valuesListSelector} from '../../../../ducks/lookup';
import {useTranslation} from "react-i18next";

const Routes = ({form, onChange, onBlur, routeActiveIndex, tabChange, settings}) => {
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const {routePoints: points = []} = form;
    const stateColors = useSelector(state => valuesListSelector(state, 'vehicleState')) || [];

    useEffect(() => {
        if (!stateColors.length) {
            dispatch(
                getLookupRequest({
                    name: 'vehicleState',
                    isForm: true,
                    isSearch: true,
                }),
            );
        }
    }, []);

    const handleChange = useCallback(
        (point, index) => {
            const newPoints = [...points];
            newPoints[index] = point;
            onChange(null, {
                name: 'routePoints',
                value: newPoints,
            });
        },
        [points],
    );

    const pointsTabs = [];

    //  console.log('form from routes.jsx', form);

    points.forEach((point, i) => {
        const state = stateColors.find(x => x.name === point.vehicleStatus);
        const color = state ? state.color : 'grey';
        pointsTabs.push({
            menuItem: {
                key: i,
                content: (
                    <label>
                        <Icon color={color} name="circle"/>
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
                        settings={settings}
                        pointChange={handleChange}
                        // onChange={onChange}
                        onBlur={onBlur}
                    />
                );
            },
        });
    });

    return (
        <div className="tabs-card">
            {pointsTabs.length > 0 ?
                <Tab
                    className="all-tabs"
                    panes={pointsTabs}
                    activeIndex={routeActiveIndex}
                    menu={{vertical: true}}
                    menuPosition="left"
                    onTabChange={tabChange}
                />
                : <label>
                    {t(`notification_emptyRoutPoints`)}
                </label>}
        </div>
    );
};

export default Routes;
